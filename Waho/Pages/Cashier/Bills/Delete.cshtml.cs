using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Bills
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public DeleteModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }

        [BindProperty]
      public Bill Bill { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FirstOrDefaultAsync(m => m.BillId == id);

            if (bill == null)
            {
                return NotFound();
            }
            else 
            {
                Bill = bill;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }
            var bill = await _context.Bills.FindAsync(id);

            if (bill != null)
            {
                Bill = bill;
                _context.Bills.Remove(Bill);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
