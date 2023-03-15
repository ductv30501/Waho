using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.DataService;

namespace Waho.Pages.Cashier
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
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            return Page();
        }
    }
}