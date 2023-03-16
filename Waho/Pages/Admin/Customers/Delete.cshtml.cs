using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Customers
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
      public Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                Customer = customer;
                Customer.Active = false;
                _context.Attach(Customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vô hiệu hoá thành công khách hàng!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
            }

            return RedirectToPage("./Index");
        }
    }
}
