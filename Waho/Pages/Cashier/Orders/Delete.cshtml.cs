using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Orders
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
      public Oder Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            if (id == null || _context.Oders == null)
            {
                return NotFound();
            }
            var order = await _context.Oders.FindAsync(id);

            if (order != null)
            {
                Order = order;
                Order.Active = false;
                _context.Attach(Order).State = EntityState.Modified; 
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xoá thành công vận đơn!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy vận đơn!";
            }

            return RedirectToPage("./Index");
        }
    }
}
