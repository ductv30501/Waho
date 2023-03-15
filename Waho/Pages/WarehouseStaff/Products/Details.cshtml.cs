using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
{
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;

        public DetailsModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }

      public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? productID)
        {
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            }
            productID = Int32.Parse(HttpContext.Request.Query["productID"]);
            if (productID == null || _context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == productID);
            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                Product = product;
            }

            return new JsonResult(product);
        }
    }
}
