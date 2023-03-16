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
        [BindProperty]
        public string Message { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            //if (!_author.IsAuthor(3))
            //{
            //    return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            //}
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Employee _employee = await _context.Employees.FindAsync(Employee.UserName);
            if (_employee == null)
            {
                Employee.WahoId = 1;
                _context.Employees.Add(Employee);
                await _context.SaveChangesAsync();
                Message = "tạo tài khoản thành công";
                return RedirectToPage("./Index");
            }
            Message = "tài khoản đã tồn tại";
            return Page();
        }
    }
}
