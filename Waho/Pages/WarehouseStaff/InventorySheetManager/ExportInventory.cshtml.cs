using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.InventorySheetManager
{
    public class ExportInventoryModel : PageModel
    {
        private readonly DataServiceManager _dataService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private InventorySheet inventorySheet;
        [BindProperty]
        public string successMessage { get; set; }
        public ExportInventoryModel(DataServiceManager dataService, IWebHostEnvironment hostingEnvironment)
        {
            _dataService = dataService;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult OnGetAsync(string _inventorySheetID)
        {
            // get inventoryDetail list
            Int32 inventorySheetID = Int32.Parse(HttpContext.Request.Query["inventorySheetID"]);
            var inventoryDetailList = _dataService.GetInventorySheetDetails(inventorySheetID);
            inventorySheet = _dataService.getInventorySheetByID(inventorySheetID);
            // create Excel package
            var package = new ExcelPackage();

            // add a new worksheet to the Excel package
            var worksheet = package.Workbook.Worksheets.Add("InventoryDetail List");

            Int64 totalDifference = 0;
            // set header row
            worksheet.Cells[1, 1].Value = "Mã phiếu kiểm kho :";
            worksheet.Cells[1, 2].Value = inventorySheetID;
            worksheet.Cells[2, 1].Value = "Người kiểm phiếu";
            worksheet.Cells[2, 2].Value = inventorySheet.UserNameNavigation.EmployeeName;
            worksheet.Cells[3, 1].Value = "Ngày tạo";
            worksheet.Cells[3, 2].Value = inventorySheet.Date.ToString();
            worksheet.Cells[5, 1].Value = "Tên sản phẩm";
            worksheet.Cells[5, 2].Value = "Số lượng thực tế trong kho";
            worksheet.Cells[5, 3].Value = "Số lượng trên web";
            worksheet.Cells[5, 4].Value = "Chênh lệch thực tế";
            worksheet.Cells[5, 5].Value = "Số tiền chênh";

            // set data rows
            int rowIndex = 6;
            foreach (var item in inventoryDetailList)
            {
                var difference = item.CurNwareHouse - item.Product.Quantity;
                var totalDiffEach = difference * item.Product.UnitInStock;
                totalDifference += totalDiffEach;
                worksheet.Cells[rowIndex, 1].Value = item.Product.ProductName;
                worksheet.Cells[rowIndex, 2].Value = item.CurNwareHouse;
                worksheet.Cells[rowIndex, 3].Value = item.Product.Quantity;
                worksheet.Cells[rowIndex, 4].Value = difference;
                worksheet.Cells[rowIndex, 5].Value = totalDiffEach;
                rowIndex++;
            }
            worksheet.Cells[4, 1].Value = "Tổng tiền chênh";
            worksheet.Cells[4, 2].Value = totalDifference +" đồng";

            // save Excel package to a file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileName = "InventoryDetail.xlsx";
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
