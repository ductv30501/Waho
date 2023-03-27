using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Waho.DataService;
using Waho.WahoModels;


namespace Waho.Pages.Cashier.Orders
{
    public class EditModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public EditModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }

        [BindProperty]
        public Oder Order { get; set; } = default!;

        public List<OderDetail> OrderDetails { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            Order = await _context.Oders.Include(b => b.UserNameNavigation).Include(b => b.Customer).Include(b => b.Shipper).FirstOrDefaultAsync(m => m.OderId == id);
            OrderDetails = await _context.OderDetails.Include(bd => bd.Product).Where(bd => bd.OderId == id).ToListAsync();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                _context.Attach(Order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sửa hoá đơn thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExist(Order.OderId))
                {
                    return NotFound();
                }
                else
                {
                    TempData["ErrorMessage"] = "Sửa hoá đơn thất bại!";
                }
            }

            return RedirectToPage("./Index");
        }

        private bool OrderExist(int id)
        {
            return _context.Oders.Any(e => e.OderId == id);
        }
    }
}
