using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.UserProfile
{
    public class UserProfileModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        public UserProfileModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Employee employee { get; set; }
        public async void OnGet(string userName)
        {
            employee =  _context.Employees.Find(userName);
        }
    }
}
