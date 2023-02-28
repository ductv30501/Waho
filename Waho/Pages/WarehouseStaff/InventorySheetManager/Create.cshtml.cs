using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
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
          

            _context.InventorySheets.Add(InventorySheet);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
