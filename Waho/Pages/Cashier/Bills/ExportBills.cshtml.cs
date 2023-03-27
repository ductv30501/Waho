using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Bills
{
    public class ExportBillsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ExportBillsModel(Waho.WahoModels.WahoContext context, Author author, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _author = author;
            _hostingEnvironment = hostingEnvironment;   
        }

        public Bill Bill { get; set; } = default!;
        public List<BillDetail> billDetails { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? billId)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            Bill = await _context.Bills.Include(b => b.UserNameNavigation).Include(b => b.Customer).FirstOrDefaultAsync(m => m.BillId == billId);
            billDetails = await _context.BillDetails.Include(bd => bd.Product).Where(bd => bd.BillId == billId).ToListAsync();

            // create Excel package
            var package = new ExcelPackage();

            // add a new worksheet to the Excel package
            var worksheet = package.Workbook.Worksheets.Add("BillDetail List");

            // set header row
            worksheet.Cells[1, 1].Value = "Mã hoá đơn:";
            worksheet.Cells[1, 2].Value = Bill.BillId;
            worksheet.Cells[2, 1].Value = "Người tạo:";
            worksheet.Cells[2, 2].Value = Bill.UserNameNavigation.EmployeeName;
            worksheet.Cells[3, 1].Value = "Ngày tạo:";
            worksheet.Cells[3, 2].Value = Bill.Date.ToString();
            worksheet.Cells[4, 1].Value = "Tình trạng:";
            worksheet.Cells[4, 2].Value = Bill.BillStatus.Equals("done") ? "Đã xong" : "Đã huỷ";
            worksheet.Cells[5, 1].Value = "Ghi chú: ";
            worksheet.Cells[5, 2].Value = Bill.Descriptions;
            

            worksheet.Cells[7, 1].Value = "Mã sản phẩm";
            worksheet.Cells[7, 2].Value = "Tên sản phẩm";
            worksheet.Cells[7, 3].Value = "Số lượng";
            worksheet.Cells[7, 4].Value = "Giảm giá(%)";
            worksheet.Cells[7, 5].Value = "Đơn giá(đ)";
            worksheet.Cells[7, 6].Value = "Tồn kho";

            worksheet.Cells[1, 4].Value = "Tên khách hàng:";
            worksheet.Cells[1, 5].Value = Bill.Customer.CustomerName;
            worksheet.Cells[2, 4].Value = "Số điện thoại:";
            worksheet.Cells[2, 5].Value = Bill.Customer.Phone;
            worksheet.Cells[3, 4].Value = "Email:";
            worksheet.Cells[3, 5].Value = Bill.Customer.Email;
            worksheet.Cells[4, 4].Value = "Mã thuế:";
            worksheet.Cells[4, 5].Value = Bill.Customer.TaxCode;

            // set data rows
            int rowIndex = 8;
            foreach (var item in billDetails)
            {
                worksheet.Cells[rowIndex, 1].Value = item.ProductId;
                worksheet.Cells[rowIndex, 2].Value = item.Product.ProductName;
                worksheet.Cells[rowIndex, 3].Value = item.Quantity;
                worksheet.Cells[rowIndex, 4].Value = item.Discount * 100;
                worksheet.Cells[rowIndex, 5].Value = item.Product.UnitPrice;
                worksheet.Cells[rowIndex, 6].Value = item.Product.Quantity;
                rowIndex++;
            }
            rowIndex++;
            worksheet.Cells[rowIndex, 5].Value = "Tổng giá tiền: ";
            worksheet.Cells[rowIndex, 6].Value = Bill.Total;

            // save Excel package to a file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileName = "BillDetail.xlsx";
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
