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
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public DetailsModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

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
    }
}
