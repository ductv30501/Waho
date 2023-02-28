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
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public DetailsModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

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
    }
}
