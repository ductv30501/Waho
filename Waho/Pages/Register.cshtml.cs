using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        public RegisterModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
        }
        [BindProperty] public Employee Employee { get; set; } = default!;
        [BindProperty] public WahoInformation WahoInformation { get; set; } = default!;
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.WahoInformations.Add(WahoInformation);
            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
