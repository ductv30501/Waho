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
        public string message { get; set; }
        public string successMessage { get; set; }
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
                message = "không tìm thấy sản phẩm";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                message = "không tìm thấy sản phẩm";
                TempData["message"] = message;
                return RedirectToPage("./Index");
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
                //message
                message = "không tìm thấy sản phẩm";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == productID);
            if (product != null)
            {
                
                Product = product;
                Product.Active = false;
                _context.Attach(Product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // message
                successMessage = "Xóa thành công sản phẩm ra khỏi danh sách";
                TempData["successMessage"] = successMessage;
            }
            message = "không tìm thấy sản phẩm";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }
    }
}
