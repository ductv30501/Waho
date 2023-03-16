using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        public ChangePasswordModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService)
        {
            _context = context;
            _dataService = dataService;
        }
        public string message { get; set; }
        [BindProperty]
        public Employee employee { get; set; }
        [BindProperty]
        public string newPassword { get; set; }
        [BindProperty]
        public string newPasswordConfirm { get; set; }
        
        public async Task OnGetAsync()
        {
            //employee = await
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Employee _employee = _dataService.GetEmployeeByUserAndPass(employee.UserName, employee.Password);
            if (_employee == null)
            {
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu ");
            }
            else
            {
                _employee.Password = newPasswordConfirm;
                _context.Attach(_employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                message = "Đổi mật khẩu thành công";
                return Page();
            }

            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
            return Page();
        }
    }
}
