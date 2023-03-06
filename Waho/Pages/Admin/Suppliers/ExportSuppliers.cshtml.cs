using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
{
    public class ExportSuppliersModel : PageModel
    {
        private readonly DataServiceManager _dataService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        [BindProperty]
        public string successMessage { get; set; }
        public ExportSuppliersModel(DataServiceManager dataService, IWebHostEnvironment hostingEnvironment)
        {
            _dataService = dataService;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult OnGetAsync(string _inventorySheetID)
        {
            // get inventoryDetail list
            var suppliers = _dataService.getSupplierList();
            // create Excel package
            var package = new ExcelPackage();

            // add a new worksheet to the Excel package
            var worksheet = package.Workbook.Worksheets.Add("InventoryDetail List");

            // set header row
            worksheet.Cells[1, 1].Value = "Tên công ty";
            worksheet.Cells[1, 2].Value = "Địa chỉ";
            worksheet.Cells[1, 3].Value = "Thành phố";
            worksheet.Cells[1, 4].Value = "Khu vực";
            worksheet.Cells[1, 5].Value = "Số điện thoại";
            worksheet.Cells[1, 6].Value = "Mã số thuế";
            worksheet.Cells[1, 7].Value = "Chi nhánh";
            worksheet.Cells[1, 8].Value = "Ghi chú";

            // set data rows
            int rowIndex = 2;
            foreach (var item in suppliers)
            {
                worksheet.Cells[rowIndex, 1].Value = item.CompanyName;
                worksheet.Cells[rowIndex, 2].Value = item.Address;
                worksheet.Cells[rowIndex, 3].Value = item.City;
                worksheet.Cells[rowIndex, 4].Value = item.Region;
                worksheet.Cells[rowIndex, 5].Value = item.Phone;
                worksheet.Cells[rowIndex, 6].Value = item.TaxCode;
                worksheet.Cells[rowIndex, 7].Value = item.Branch;
                worksheet.Cells[rowIndex, 8].Value = item.Description;
                rowIndex++;
            }

            // save Excel package to a file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileName = "Supplier.xlsx";
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
