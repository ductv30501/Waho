using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff
{
    public class IndexModel : PageModel
    {
        
        private readonly Author _author;
        public IndexModel(Author author)
        {
            _author = author;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản sản phẩm" });
            }
            
            return Page();
        }
    }
}