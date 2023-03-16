using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Waho.Pages
{
    public class LogOutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            HttpContext.Session.Clear();
            return Redirect("/Index");
        }
    }
}
