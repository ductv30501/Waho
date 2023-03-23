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
using System.Diagnostics;
using Newtonsoft.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Waho.Pages.Cashier.Bills
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

        [BindProperty]
        public Customer Customer { get; set; } = default!;

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

        private Employee employee { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }
            return Page();
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

            string customerId = HttpContext.Request.Form["customerId"];
            string total = HttpContext.Request.Form["total"];
            string listBillDetail = HttpContext.Request.Form["listBillDetail"];
            List<BillDetail> billDetails = JsonConvert.DeserializeObject<List<BillDetail>>(listBillDetail);

            var employeeJson = HttpContext.Session.GetString("Employee");

            if (employeeJson != null)
            {
                employee = JsonSerializer.Deserialize<Employee>(employeeJson);
            }

            if (!string.IsNullOrEmpty(customerId))
            {
                Bill.CustomerId = int.Parse(customerId);
            }
            else
            {
                _context.Customers.Add(Customer);
                int resultAddCustomer = _context.SaveChanges();
                if (resultAddCustomer != 0)
                {
                    Bill.CustomerId = Customer.CustomerId;
                }
                
            }

            Bill.UserName = employee.UserName;
            Bill.Date = DateTime.Now;
            Bill.Active = true;
            Bill.BillStatus = "done";
            Bill.Total = decimal.Parse(total);


            _context.Bills.Add(Bill);
            int result = _context.SaveChanges();
            if (result != 0)
            {
                foreach (var billDetail in billDetails)
                {
                    // Thiết lập giá trị BillId cho bản ghi BillDetail
                    billDetail.BillId = Bill.BillId;
                }

                // Thêm bản ghi BillDetail vào context
                _context.BillDetails.AddRange(billDetails);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo hoá đơn thành công!";
            }


            return RedirectToPage("./Index");
        }
    }
}
