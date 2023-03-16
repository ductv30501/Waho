using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Waho.Pages
{
    public class accessDeniedModel : PageModel
    {
        public string ReturnUrl { get; set; }
        public void OnGet(string message)
        {
            ReturnUrl = message;
        }
    }
}
