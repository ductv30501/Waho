using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
{
    public class UploadFileExcelModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly IFileProvider _fileProvider;
        public string message { get; set; }
        public string successMessage { get; set; }
        public UploadFileExcelModel(Waho.WahoModels.WahoContext context, IFileProvider fileProvider)
        {
            _context = context;
            _fileProvider = fileProvider;
        }

        [BindProperty]
        public List<Product> Products { get; set; }
        [BindProperty]
        public string ExcelFile { get; set; }


        public IActionResult OnGetAsync()
        {
            // Lấy thông tin file từ IFileProvider
            IFileInfo fileInfo = _fileProvider.GetFileInfo("Products.xlsx");
            if (fileInfo.Exists)
            {
                // Đọc nội dung file và trả về file download
                var stream = new MemoryStream();
                using (var fileStream = fileInfo.CreateReadStream())
                {
                    fileStream.CopyTo(stream);
                }
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");
            }
            else
            {
                // messagse
                message = "file không tồn tại";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var req = HttpContext.Request;
            if (string.IsNullOrEmpty(req.Form["ExcelFile"]))
            {
                // messagse
                message = "đường dẫn file trống, null";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            else
            {

                IFileInfo fileInfo = _fileProvider.GetFileInfo(req.Form["ExcelFile"]);
                ExcelFile = fileInfo.PhysicalPath;
            }
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                // messagse
                message = "hãy chọn file trước khi gửi";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            else
            {
                try
                {
                    var products = ReadExcel(ExcelFile);
                    if (products == null)
                    {
                        // messagse
                        message = "trong file không có sản phẩm nào";
                        TempData["message"] = message;
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        await _context.BulkInsertAsync(products);
                        successMessage = $"{products.Count} sản phẩm được thêm vào thành công";
                        TempData["successMessage"] = successMessage;
                        return RedirectToPage("./Index");
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Page();


        }
        private List<Product> ReadExcel(string path)
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = package.Workbook.Worksheets[0];
            var products = new List<Product>();
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var product = new Product
                {
                    ProductName = worksheet.Cells[row, 1].GetValue<string>(),
                    ImportPrice = worksheet.Cells[row, 2].GetValue<int>(),
                    UnitPrice = worksheet.Cells[row, 3].GetValue<int>(),
                    UnitInStock = worksheet.Cells[row, 4].GetValue<int>(),
                    HaveDate = worksheet.Cells[row, 5].GetValue<bool>(),
                    DateOfManufacture = worksheet.Cells[row, 6].GetValue<DateTime>(),
                    Expiry = worksheet.Cells[row, 7].GetValue<DateTime>(),
                    Trademark = worksheet.Cells[row, 8].GetValue<string>(),
                    Weight = worksheet.Cells[row, 9].GetValue<int>(),
                    Location = worksheet.Cells[row, 10].GetValue<string>(),
                    Unit = worksheet.Cells[row, 11].GetValue<string>(),
                    InventoryLevelMin = worksheet.Cells[row, 12].GetValue<int>(),
                    InventoryLevelMax = worksheet.Cells[row, 13].GetValue<int>(),
                    Description = worksheet.Cells[row, 14].GetValue<string>(),
                    SubCategoryId = worksheet.Cells[row, 15].GetValue<int>(),
                    SupplierId = worksheet.Cells[row, 16].GetValue<int>(),
                    Active = worksheet.Cells[row, 17].GetValue<bool>()
                };

                products.Add(product);
            }

            return products;
        }

    }
}
