using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Employees
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public DeleteModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }
        public string message { get; set; }
        public string successMessage { get; set; }
        [BindProperty]
      public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }

            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.UserName == id);

            if (employee != null)
            {
                Employee= employee;
                Employee.Active = false;
                _context.Attach(Employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // message
                successMessage = "Xóa thành công nhân viên ra khỏi danh sách";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            message = "không tìm thấy nhân viên";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }

    }
}
