using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Employees
{
    public class EditModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public EditModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }
        public string message { get; set; }
        public string successMessage { get; set; }
        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            return Page();
        }

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

            Employee _Employee = _context.Employees.Find(raw_userName);
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
            if (string.IsNullOrEmpty(req.Form["activeUpdate"]))
            {
                _Employee.Active = false;
            }
            else
            {
                _Employee.Active = true;
            }
            _Employee.Email = raw_email;
            _Employee.Note = raw_note;
            _Employee.Phone= raw_phone;
            _Employee.Address = raw_addrress;
            _Employee.Region= raw_region;
            _Employee.Role = Int32.Parse(raw_role);
            
            //update to data
            _context.Attach(_Employee).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(_Employee.UserName))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            successMessage = "Đã sửa thành công thông tin nhân viên";
            TempData["successMessage"] = successMessage;
            return RedirectToPage("./Index");
        }

        private bool EmployeeExists(string id)
        {
          return _context.Employees.Any(e => e.UserName == id);
        }
    }
}
