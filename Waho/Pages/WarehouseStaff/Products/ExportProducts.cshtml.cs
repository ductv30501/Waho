using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
{
    public class ExportProductsModel : PageModel
    {
        private readonly DataServiceManager _dataService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Author _author;
        public string successMessage { get; set; }
        public ExportProductsModel(DataServiceManager dataService, IWebHostEnvironment hostingEnvironment, Author author)
        {
            _dataService = dataService;
            _hostingEnvironment = hostingEnvironment;
            _author = author;
        }
        public  IActionResult OnGetAsync() {
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            }
            // get product list
            var productList = _dataService.GetProductsByCateID(1);

            // create Excel package
            var package = new ExcelPackage();

            // add a new worksheet to the Excel package
            var worksheet = package.Workbook.Worksheets.Add("Product List");

            // set header row
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Tên sản phẩm";
            worksheet.Cells[1, 3].Value = "Giá nhập";
            worksheet.Cells[1, 4].Value = "Đơn giá";
            worksheet.Cells[1, 5].Value = "Giá tại cửa hàng";
            worksheet.Cells[1, 6].Value = "Có HSD ?";
            worksheet.Cells[1, 7].Value = "Ngày sản xuất";
            worksheet.Cells[1, 8].Value = "Hạn sử dụng";
            worksheet.Cells[1, 9].Value = "Thương hiệu";
            worksheet.Cells[1, 10].Value = "Trọng lượng";
            worksheet.Cells[1, 11].Value = "Vị trí";
            worksheet.Cells[1, 12].Value = "Đơn vị";
            worksheet.Cells[1, 13].Value = "Đinh mức tồn min";
            worksheet.Cells[1, 14].Value = "Đinh mức tồn max";
            worksheet.Cells[1, 15].Value = "Miêu tả";
            worksheet.Cells[1, 16].Value = "Nhóm hàng";
            worksheet.Cells[1, 17].Value = "Nhà cung cấp";
            worksheet.Cells[1, 18].Value = "active";
            worksheet.Cells[1, 19].Value = "Số lượng";

            // set data rows
            int rowIndex = 2;
            foreach (var product in productList)
            {
                worksheet.Cells[rowIndex, 1].Value = product.ProductId;
                worksheet.Cells[rowIndex, 2].Value = product.ProductName;
                worksheet.Cells[rowIndex, 3].Value = product.ImportPrice +" đồng";
                worksheet.Cells[rowIndex, 4].Value = product.UnitPrice + " đồng";
                worksheet.Cells[rowIndex, 5].Value = product.UnitInStock + " đồng";
                worksheet.Cells[rowIndex, 6].Value = (product.HaveDate == true ? "Có HSD" : "Không có HSD");
                worksheet.Cells[rowIndex, 7].Value = product.DateOfManufacture.ToString();
                worksheet.Cells[rowIndex, 8].Value = product.Expiry.ToString();
                worksheet.Cells[rowIndex, 9].Value = product.Trademark;
                worksheet.Cells[rowIndex, 10].Value = product.Weight;
                worksheet.Cells[rowIndex, 11].Value = product.Location;
                worksheet.Cells[rowIndex, 12].Value = product.Unit;
                worksheet.Cells[rowIndex, 13].Value = product.InventoryLevelMin;
                worksheet.Cells[rowIndex, 14].Value = product.InventoryLevelMax;
                worksheet.Cells[rowIndex, 15].Value = product.Description;
                worksheet.Cells[rowIndex, 16].Value = product.SubCategory.SubCategoryName;
                worksheet.Cells[rowIndex, 17].Value = product.Supplier.CompanyName;
                worksheet.Cells[rowIndex, 18].Value = (product.Active == true ? "Đang có" : "Không có");
                worksheet.Cells[rowIndex, 19].Value = product.Quantity;
                rowIndex++;
            }

            // save Excel package to a file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileName = "ProductList.xlsx";
            string filePath = Path.Combine(webRootPath, fileName);
            FileInfo file = new FileInfo(filePath);
            package.SaveAs(file);

            // return file download
            var fileProvider = new PhysicalFileProvider(webRootPath);
            var fileInfo = fileProvider.GetFileInfo(fileName);
            var fileStream = fileInfo.CreateReadStream();
            successMessage = "in ra file excel thành công";
            TempData["successMessage"] = successMessage;
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }   
    }

}

