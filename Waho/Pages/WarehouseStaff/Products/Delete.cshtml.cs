using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public DeleteModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? ID)
        {
            Console.WriteLine(HttpContext.Request.Form["productID"] + "haiz");
            //int productID = Int32.Parse(HttpContext.Request.Form["productID"]);
            int productID = Int32.Parse(ID);
            if (productID == null || _context.Products == null)
            {
                Console.WriteLine(productID + "null roi ");
                return NotFound();
            }
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == productID);
            if (product != null)
            {
                Console.WriteLine(productID + "null roi ");
                Product = product;
                Product.Active = false;
                _context.Attach(Product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
