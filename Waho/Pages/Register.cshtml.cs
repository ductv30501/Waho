using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.WahoModels;

namespace Waho.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty] public Employee Employee { get; set; } = default!;
        [BindProperty] public WahoInformation WahoInformation { get; set; } = default!;
        public void OnGet()
        {
        }
    }
}
