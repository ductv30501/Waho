using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Bills
{
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public IndexModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        public IList<Bill> Bill { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Bills != null)
            {
                Bill = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.UserNameNavigation).ToListAsync();
            }
        }
    }
}
