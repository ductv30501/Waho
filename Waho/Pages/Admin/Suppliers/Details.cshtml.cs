using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
{
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public DetailsModel(Waho.WahoModels.WahoContext context , Author author)
        {
            _context = context;
            _author = author;
        }

      public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnGetAsync(int? supplierID)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }

            supplierID = Int32.Parse(HttpContext.Request.Query["supplierID"]);
            if (supplierID == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.SupplierId == supplierID);
            if (supplier == null)
            {
                return NotFound();
            }
            else 
            {
                Supplier = supplier;
            }
            return new JsonResult(supplier);
        }
    }
}
