using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.InventorySheetManager
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        public string message { get; set; }
        public string successMessage { get; set; }
        public DeleteModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InventorySheet InventorySheet { get; set; }

        public async Task<IActionResult> OnGetAsync(string? inventorySheetID)
        {
            int inventorySheetId = Int32.Parse(inventorySheetID);
            var _InventorySheet = await _context.InventorySheets.FirstOrDefaultAsync(m => m.InventorySheetId == inventorySheetId);
            if (_InventorySheet != null)
            {
                InventorySheet = _InventorySheet;
                InventorySheet.Active = false;
                _context.Attach(InventorySheet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // message
                successMessage = "Xóa thành công phiếu kiểm kho ra khỏi danh sách";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            message = "không tìm thấy phiếu kiểm kho";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }


    }
}
