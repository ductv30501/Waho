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

        public DeleteModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
      public InventorySheetDetail InventorySheetDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.InventorySheetDetails == null)
            {
                return NotFound();
            }

            var inventorysheetdetail = await _context.InventorySheetDetails.FirstOrDefaultAsync(m => m.InventorySheetId == id);

            if (inventorysheetdetail == null)
            {
                return NotFound();
            }
            else 
            {
                InventorySheetDetail = inventorysheetdetail;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.InventorySheetDetails == null)
            {
                return NotFound();
            }
            var inventorysheetdetail = await _context.InventorySheetDetails.FindAsync(id);

            if (inventorysheetdetail != null)
            {
                InventorySheetDetail = inventorysheetdetail;
                _context.InventorySheetDetails.Remove(InventorySheetDetail);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
