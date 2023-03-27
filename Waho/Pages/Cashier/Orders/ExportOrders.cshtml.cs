using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Orders
{
    public class ExportOrdersModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ExportOrdersModel(Waho.WahoModels.WahoContext context, Author author, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _author = author;
            _hostingEnvironment = hostingEnvironment;
        }

        public Oder Order { get; set; } = default!;
        public List<OderDetail> OrderDetails { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? orderId)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            Order = await _context.Oders.Include(b => b.UserNameNavigation).Include(b => b.Customer).Include(b => b.Shipper).FirstOrDefaultAsync(m => m.OderId == orderId);
            OrderDetails = await _context.OderDetails.Include(bd => bd.Product).Where(bd => bd.OderId == orderId).ToListAsync();

            // create Excel package
            var package = new ExcelPackage();

            // add a new worksheet to the Excel package
            var worksheet = package.Workbook.Worksheets.Add("BillDetail List");

            // set header row
            worksheet.Cells[1, 1].Value = "Mã vận đơn:";
            worksheet.Cells[1, 2].Value = Order.OderId;
            worksheet.Cells[2, 1].Value = "Người tạo:";
            worksheet.Cells[2, 2].Value = Order.UserNameNavigation.EmployeeName;
            worksheet.Cells[3, 1].Value = "Ngày tạo:";
            worksheet.Cells[3, 2].Value = Order.OrderDate.ToString();
            worksheet.Cells[3, 1].Value = "Ngày dự kiến giao:";
            worksheet.Cells[3, 2].Value = Order.EstimatedDate.ToString();
            worksheet.Cells[4, 1].Value = "Tình trạng:";

            switch (Order.OderState)
            {
                case "notDelivery":
                    worksheet.Cells[4, 2].Value = "Chưa giao hàng";
                    break;
                case "pending":
                    worksheet.Cells[4, 2].Value = "Đang giao hàng";
                    break;
                case "done":
                    worksheet.Cells[4, 2].Value = "Đã giao hàng";
                    break;
                case "cantDelivery":
                    worksheet.Cells[4, 2].Value = "không giao được";
                    break;
                case "cancel":
                    worksheet.Cells[4, 2].Value = "Đã huỷ";
                    break;
            }

            worksheet.Cells[5, 1].Value = "Khu vực: ";
            worksheet.Cells[5, 2].Value = Order.Region;
            worksheet.Cells[6, 1].Value = "Mã COD: ";
            worksheet.Cells[6, 2].Value = Order.Cod;

            worksheet.Cells[8, 1].Value = "Mã sản phẩm";
            worksheet.Cells[8, 2].Value = "Tên sản phẩm";
            worksheet.Cells[8, 3].Value = "Số lượng";
            worksheet.Cells[8, 4].Value = "Giảm giá(%)";
            worksheet.Cells[8, 5].Value = "Đơn giá(đ)";
            worksheet.Cells[8, 6].Value = "Tồn kho";

            worksheet.Cells[1, 4].Value = "Tên khách hàng:";
            worksheet.Cells[1, 5].Value = Order.Customer.CustomerName;
            worksheet.Cells[2, 4].Value = "Số điện thoại:";
            worksheet.Cells[2, 5].Value = Order.Customer.Phone;
            worksheet.Cells[3, 4].Value = "Email:";
            worksheet.Cells[3, 5].Value = Order.Customer.Email;
            worksheet.Cells[4, 4].Value = "Mã thuế:";
            worksheet.Cells[4, 5].Value = Order.Customer.TaxCode;

            worksheet.Cells[1, 7].Value = "Tên người giao:";
            worksheet.Cells[1, 8].Value = Order.Shipper.ShipperName;
            worksheet.Cells[2, 7].Value = "Số điện thoại:";
            worksheet.Cells[2, 8].Value = Order.Shipper.Phone;


            // set data rows
            int rowIndex = 9;
            foreach (var item in OrderDetails)
            {
                worksheet.Cells[rowIndex, 1].Value = item.ProductId;
                worksheet.Cells[rowIndex, 2].Value = item.Product.ProductName;
                worksheet.Cells[rowIndex, 3].Value = item.Quantity;
                worksheet.Cells[rowIndex, 4].Value = item.Discount * 100;
                worksheet.Cells[rowIndex, 5].Value = item.Product.UnitPrice;
                worksheet.Cells[rowIndex, 6].Value = item.Product.Quantity;
                rowIndex++;
            }
            rowIndex ++;
            worksheet.Cells[rowIndex, 5].Value = "Tổng giá tiền: ";
            worksheet.Cells[rowIndex, 6].Value = Order.Total + Order.Deposit;
            rowIndex ++;
            worksheet.Cells[rowIndex, 5].Value = "Số tiền đã trả: ";
            worksheet.Cells[rowIndex, 6].Value = Order.Deposit;
            rowIndex++;
            worksheet.Cells[rowIndex, 5].Value = "Phải trả: ";
            worksheet.Cells[rowIndex, 6].Value = Order.Total;

            // save Excel package to a file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileName = "OrderDetail.xlsx";
            string filePath = Path.Combine(webRootPath, fileName);
            FileInfo file = new FileInfo(filePath);
            package.SaveAs(file);

            // return file download
            var fileProvider = new PhysicalFileProvider(webRootPath);
            var fileInfo = fileProvider.GetFileInfo(fileName);
            var fileStream = fileInfo.CreateReadStream();
            TempData["SuccessMessage-export"] = "in ra file excel thành công";

            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
