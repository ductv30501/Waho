using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.ReturnOrders
{
    public class ExportReturnOrderDetailModel : PageModel
    {
        private readonly DataServiceManager _dataService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Author _author;
        private ReturnOrder returnOrder;
        [BindProperty]
        public string successMessage { get; set; }
        public ExportReturnOrderDetailModel(DataServiceManager dataService, IWebHostEnvironment hostingEnvironment, Author author)
        {
            _dataService = dataService;
            _hostingEnvironment = hostingEnvironment;
            _author = author;
        }
        public IActionResult OnGetAsync(int returnOrderID)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }
            // get inventoryDetail list
            var inventoryDetailList = _dataService.GetReturnOrderDetails(returnOrderID);
            returnOrder = _dataService.getReturnOrderByID(returnOrderID);

            // create Excel package
            var package = new ExcelPackage();

            // add a new worksheet to the Excel package
            var worksheet = package.Workbook.Worksheets.Add("ReturnOrderDetail List");

            // set header row
            worksheet.Cells[1, 1].Value = "Mã hoàn đơn :";
            worksheet.Cells[1, 2].Value = returnOrderID;
            worksheet.Cells[1, 4].Value = "Tên khách hàng";
            worksheet.Cells[1, 5].Value = returnOrder.Customer.CustomerName;
            worksheet.Cells[2, 1].Value = "Người tạo đơn";
            worksheet.Cells[2, 2].Value = returnOrder.UserNameNavigation.EmployeeName;
            worksheet.Cells[3, 1].Value = "Ngày tạo";
            worksheet.Cells[3, 2].Value = returnOrder.Date.ToString();
            worksheet.Cells[4, 1].Value = "Số tiền cần trả khách";
            worksheet.Cells[4, 2].Value = returnOrder.PayCustomer + " đồng";
            worksheet.Cells[4, 4].Value = "Số tiền đã trả khách";
            worksheet.Cells[4, 5].Value = returnOrder.PaidCustomer + " đồng";
            worksheet.Cells[5, 1].Value = "Tên sản phẩm";
            worksheet.Cells[5, 2].Value = "Thương hiệu";
            worksheet.Cells[5, 3].Value = "Giá tại kho";
            worksheet.Cells[5, 4].Value = "Số lượng";

            // set data rows
            int rowIndex = 6;
            foreach (var item in inventoryDetailList)
            {
                worksheet.Cells[rowIndex, 1].Value = item.Product.ProductName;
                worksheet.Cells[rowIndex, 2].Value = item.Product.Trademark;
                worksheet.Cells[rowIndex, 3].Value = item.Product.UnitInStock;
                worksheet.Cells[rowIndex, 4].Value = item.Quantity;
                rowIndex++;
            }
            

            // save Excel package to a file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileName = "ReturnOrderDetail.xlsx";
            string filePath = Path.Combine(webRootPath, fileName);
            FileInfo file = new FileInfo(filePath);
            package.SaveAs(file);

            // return file download
            var fileProvider = new PhysicalFileProvider(webRootPath);
            var fileInfo = fileProvider.GetFileInfo(fileName);
            var fileStream = fileInfo.CreateReadStream();
            successMessage = "in ra file excel thành công";
            //TempData["successMessage"] = successMessage;
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
