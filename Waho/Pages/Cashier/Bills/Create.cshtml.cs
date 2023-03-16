using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Bills
{
    public class CreateModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public CreateModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public Bill bill { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<BillDetail> billDetails { get; set; }

        [BindProperty(SupportsGet = true)]
        public BillDetail billDetail { get; set; }


        [BindProperty(SupportsGet = true)]
        public List<Product> products { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Customer> customers { get; set; }

        [BindProperty]
        public Bill Bill { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnGetProducts(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || string.IsNullOrEmpty(q))
            {
                return new JsonResult("");
            }
            else
            {
                products = await _context.Products.Where(p => (p.ProductName.ToLower().Contains(q.ToLower()) || p.ProductId.ToString().Contains(q)))
                    .Where(p => p.Active == true)
                    .Where(p => p.Quantity > 0)
                    .Take(5).ToListAsync();
                return new JsonResult(products);
            }
        }


        public async Task<IActionResult> OnGetCustomers(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || string.IsNullOrEmpty(q))
            {
                return new JsonResult("");
            }
            else
            {
                customers = await _context.Customers.Where(c => (c.CustomerName.ToLower().Contains(q.ToLower()) || c.Phone.Contains(q)))
                    .Where(c => c.Active == true)
                    .Take(5).ToListAsync();
                return new JsonResult(customers);
            }
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string customerId = HttpContext.Request.Form["customerId"];

            // Lấy giá trị từ session
            //string employeeJson = HttpContext.Session.GetString("Employee");

            //// Chuyển đổi chuỗi JSON thành đối tượng Employee
            //    Employee employee = JsonSerializer.Deserialize<Employee>(employeeJson);

            //// Sử dụng giá trị đối tượng Employee
            //if (employee != null)
            //{
            //    // ...
            //}

            _context.Bills.Add(Bill);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
