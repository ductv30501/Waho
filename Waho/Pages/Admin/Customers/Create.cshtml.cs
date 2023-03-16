using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Customers
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            string name = HttpContext.Request.Form["name"];
            string raw_dob = HttpContext.Request.Form["dob"];
            string phone = HttpContext.Request.Form["phone"];
            string email = HttpContext.Request.Form["email"];
            string raw_type = HttpContext.Request.Form["type"];
            string tax = HttpContext.Request.Form["tax"];
            string address = HttpContext.Request.Form["address"];
            string note = HttpContext.Request.Form["note"];

            Customer.CustomerName = name;
            Customer.Adress = address;
            Customer.Phone = phone;
            Customer.Email = email;
            Customer.Active = true;
            Customer.Description = note;
            Customer.TaxCode = tax;
            Customer.Dob = DateTime.Parse(raw_dob);
            Customer.TypeOfCustomer = Boolean.Parse(raw_type);

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo thông tin khách hàng thành công!";
            return RedirectToPage("./Index");
        }
    }
}
