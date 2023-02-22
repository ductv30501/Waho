using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Ships
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public DeleteModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Shipper Shipper { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers.FirstOrDefaultAsync(m => m.ShipperId == id);

            if (shipper == null)
            {
                return NotFound();
            }
            else 
            {
                Shipper = shipper;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }
            var shipper = await _context.Shippers.FindAsync(id);

            if (shipper != null)
            {
                Shipper = shipper;
                _context.Shippers.Remove(Shipper);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
