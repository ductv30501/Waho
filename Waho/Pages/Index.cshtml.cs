using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (_dataService.GetEmployeeByUserAndPass(Employee.UserName, Employee.Password) != null)
            {
                //HttpContext.Session.SetString("",Employee);
                return RedirectToPage("./Admin/Index");
            }
            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
            return Page();
        }
    }
}