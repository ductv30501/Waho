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
            var bill = await _context.Bills.FindAsync(id);

            if (bill != null)
            {
                Bill = bill;
                Bill.Active = false;
                _context.Attach(Bill).State = EntityState.Modified; 
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xoá thành công hoá đơn!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy hoá đơn!";
            }

            return RedirectToPage("./Index");
        }
    }
}
