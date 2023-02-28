using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.InventorySheetManager
{
    public class EditModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public EditModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InventorySheetDetail InventorySheetDetail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.InventorySheetDetails == null)
            {
                return NotFound();
            }

            var inventorysheetdetail =  await _context.InventorySheetDetails.FirstOrDefaultAsync(m => m.InventorySheetId == id);
            if (inventorysheetdetail == null)
            {
                return NotFound();
            }
            InventorySheetDetail = inventorysheetdetail;
           ViewData["InventorySheetId"] = new SelectList(_context.InventorySheets, "InventorySheetId", "UserName");
           ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(InventorySheetDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventorySheetDetailExists(InventorySheetDetail.InventorySheetId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool InventorySheetDetailExists(int id)
        {
          return _context.InventorySheetDetails.Any(e => e.InventorySheetId == id);
        }
    }
}
