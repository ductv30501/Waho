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

        
        public async Task<IActionResult> OnGetAsync(string? productID)
        {
            int productId = Int32.Parse(productID);
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == productId);
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
