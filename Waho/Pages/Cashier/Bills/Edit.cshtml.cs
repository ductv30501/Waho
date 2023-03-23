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


namespace Waho.Pages.Cashier.Bills
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
        public Bill Bill { get; set; } = default!;

        public List<BillDetail> billDetails { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            Bill = await _context.Bills.Include(b => b.UserNameNavigation).Include(b => b.Customer).FirstOrDefaultAsync(m => m.BillId == id);
            billDetails = await _context.BillDetails.Include(bd => bd.Product).Where(bd => bd.BillId == id).ToListAsync();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                _context.Attach(Bill).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sửa hoá đơn thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(Bill.BillId))
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

        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.BillId == id);
        }
    }
}
