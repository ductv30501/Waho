using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text.Json;
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
            Employee _employee = _dataService.GetEmployeeByUserAndPass(Employee.UserName, Employee.Password);
            if (_employee != null)
            {
                HttpContext.Session.SetString("Employee", JsonSerializer.Serialize(_employee));
                switch (_employee.Role)
                {
                    case 1:
                        return RedirectToPage("./Admin/Index");
                    case 2: 
                        return RedirectToPage("./Cashier/Bills/Index");
                    case 3:
                        return RedirectToPage("./WarehouseStaff/Products/Index");
                }
                
            }
            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
            return Page();
        }
    }
}