using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.InventorySheetManager
{
    public class CreateModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly IFileProvider _fileProvider;
        public string message { get; set; }
        public string successMessage { get; set; }
        public CreateModel(Waho.WahoModels.WahoContext context, IFileProvider fileProvider)
        {
            _context = context;
            _fileProvider = fileProvider;
        }


        [BindProperty]
        public InventorySheet InventorySheet { get; set; }
        List<InventorySheetDetail> inventorySheetDetails { get; set; }
        [BindProperty]
        public string ExcelFile { get; set; }
        private int inventoryID;
        public IActionResult OnGetAsync()
        {
            // Lấy thông tin file từ IFileProvider
            IFileInfo fileInfo = _fileProvider.GetFileInfo("Inventory.xlsx");
            if (fileInfo.Exists)
            {
                // Đọc nội dung file và trả về file download
                var stream = new MemoryStream();
                using (var fileStream = fileInfo.CreateReadStream())
                {
                    fileStream.CopyTo(stream);
                }
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "templateUploadInventory.xlsx");
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
            //get data form form submit 
            string raw_employeeID = req.Form["employeeID"];
            string raw_date = req.Form["date"];
            string raw_description = req.Form["description"];
            if (!string.IsNullOrEmpty(raw_employeeID))
            {
                InventorySheet.UserName= raw_employeeID;
            }
            if (string.IsNullOrWhiteSpace(raw_date))
            {
                // messagse
                message = "ngày kiểm kho không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            InventorySheet.Date = DateTime.Parse(raw_date);
            if (!string.IsNullOrEmpty(raw_description))
            {
                InventorySheet.Description = raw_description;
            }
            
            //add file product 
            if (string.IsNullOrEmpty(req.Form["ExcelFile"]))
            {
                // messagse
                message = "đường dẫn file trống, bạn cần chọn file trước khi gửi form";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            else
            {
                // add inventory sheet
                _context.InventorySheets.Add(InventorySheet);
                await _context.SaveChangesAsync();
                inventoryID = InventorySheet.InventorySheetId;
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
                    var inventorySheetDetails = ReadExcel(ExcelFile);
                    if (inventorySheetDetails == null)
                    {
                        // messagse
                        message = "trong file không có sản phẩm nào";
                        TempData["message"] = message;
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        await _context.BulkInsertAsync(inventorySheetDetails);
                        successMessage = $"{inventorySheetDetails.Count} sản phẩm được thêm vào phiếu kiểm thành công";
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
        private List<InventorySheetDetail> ReadExcel(string path)
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = package.Workbook.Worksheets[0];
            var inventorySheetDetails = new List<InventorySheetDetail>();
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var inventoryDetail = new InventorySheetDetail
                {
                    InventorySheetId = inventoryID,
                    ProductId = worksheet.Cells[row, 1].GetValue<int>(),
                    CurNwareHouse = worksheet.Cells[row, 19].GetValue<int>()
                };

                inventorySheetDetails.Add(inventoryDetail);
            }

            return inventorySheetDetails;
        }
    }
}
