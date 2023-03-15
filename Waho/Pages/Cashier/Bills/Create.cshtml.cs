using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Bills
{
    public class CreateModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public CreateModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Bill bill { get; set; }

        [BindProperty]
        public List<BillDetail> billDetails { get; set; }

        [BindProperty]
        public List<Product> products { get; set; }


        public void OnGet()
        {

        }
        
        public async Task<IActionResult> OnGetProducts(string? q)
        {
            if (!string.IsNullOrEmpty(q.Trim()))
            {
                products = _context.Products.Where(p => p.ProductName.ToLower().Contains(q.ToLower())).Take(10).ToList();
            }

            return new JsonResult(products);
        }

        [BindProperty]
        public Bill Bill { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Bills.Add(Bill);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
