using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
{
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public IndexModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        public IList<Supplier> Supplier { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Suppliers != null)
            {
                Supplier = await _context.Suppliers.ToListAsync();
            }
        }
    }
}
