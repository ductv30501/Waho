using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Employees
{
    public class CreateModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;

        public CreateModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            return Page();
        }
        public string message { get; set; }
        public string successMessage { get; set; }
        [BindProperty]
        public Employee Employee { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var req = HttpContext.Request;
            //get data form form submit 
            string raw_userName = req.Form["userName"];
            string raw_EmployeeName = req.Form["employeeName"];
            string raw_title = req.Form["title"];
            string raw_dob = req.Form["dob"];
            string raw_hireDate = req.Form["hireDate"];
            string raw_phone = req.Form["phone"];
            string raw_addrress = req.Form["addrress"];
            string raw_region = req.Form["region"];
            string raw_email = req.Form["email"];
            string raw_note = req.Form["note"];
            string raw_role = req.Form["role"];

            Employee _Employee = new Employee();
            if (string.IsNullOrEmpty(raw_userName) || raw_userName.Length < 6)
            {
                message = "Bạn nhập tài khoản có ít nhất 6 ký tự";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            _Employee.UserName = raw_userName;
            Employee _employee = await _context.Employees.FindAsync(_Employee.UserName);
            if (_employee == null)
            {
                _Employee.EmployeeName = raw_EmployeeName;
                _Employee.Title = raw_title;
                if (string.IsNullOrEmpty(raw_dob))
                {
                    message = "Bạn điền ngày sinh";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
                _Employee.Dob = DateTime.Parse(raw_dob);
                if (!string.IsNullOrEmpty(raw_hireDate))
                {
                    _Employee.Dob = DateTime.Parse(raw_hireDate);
                }

                if (string.IsNullOrEmpty(raw_email))
                {
                    message = "Bạn điền email của nhân viên";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
                if (string.IsNullOrEmpty(req.Form["activeInsert"]))
                {
                    _Employee.Active = false;
                }
                else
                {
                    _Employee.Active = true;
                }
                _Employee.Email = raw_email;
                _Employee.Note = raw_note;
                _Employee.Phone = raw_phone;
                _Employee.Address = raw_addrress;
                _Employee.Region = raw_region;
                _Employee.Password = "wahoEmployee";
                _Employee.Role = Int32.Parse(raw_role);
                _Employee.WahoId = 1;

                _context.Employees.Add(_Employee);
                await _context.SaveChangesAsync();
                successMessage = "tạo tài khoản thành công, mật khẩu mặc định: wahoEmployee";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            TempData["message"] = "tên tài khoản đã tồn tại";
            return RedirectToPage("./Index");
        }
    }
}
