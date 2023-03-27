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

namespace Waho.Pages.Cashier.Orders
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
        public OderDetail OrderDetail { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Product> products { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Shipper> Shippers { get; set; }

        [BindProperty]
        public Oder Order { get; set; }

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

        public async Task<IActionResult> OnGetShippers(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || string.IsNullOrEmpty(q))
            {
                return new JsonResult("");
            }
            else
            {
                Shippers = await _context.Shippers.Where(s => (s.ShipperName.ToLower().Contains(q.ToLower()) || s.Phone.Contains(q)))
                    .Take(5).ToListAsync();
                return new JsonResult(Shippers);
            }
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            string customerId = HttpContext.Request.Form["customerId"];
            string estDate = HttpContext.Request.Form["estDate"];
            string cod = HttpContext.Request.Form["cod"];
            string region = HttpContext.Request.Form["region"];
            string shipperId = HttpContext.Request.Form["shipperId"];
            string payed = HttpContext.Request.Form["payed"];
            string total = HttpContext.Request.Form["total"];
            string listOrderDetail = HttpContext.Request.Form["listOrderDetail"];
            List<OderDetail> orderDetails = JsonConvert.DeserializeObject<List<OderDetail>>(listOrderDetail);

            var employeeJson = HttpContext.Session.GetString("Employee");

            if (employeeJson != null)
            {
                employee = JsonSerializer.Deserialize<Employee>(employeeJson);
            }

            if (!string.IsNullOrEmpty(customerId))
            {
                Order.CustomerId = int.Parse(customerId);
            }
            else
            {
                string name = HttpContext.Request.Form["name"];
                string phone = HttpContext.Request.Form["phone"];
                string dob = HttpContext.Request.Form["dob"];
                string email = HttpContext.Request.Form["email"];
                string type = HttpContext.Request.Form["type"];
                string tax = HttpContext.Request.Form["tax"];
                string address = HttpContext.Request.Form["address"];
                string description = HttpContext.Request.Form["description"];

                Customer.CustomerName = name;
                Customer.Adress = address;
                Customer.Description = description;
                Customer.Email = email;
                Customer.Phone = phone;
                Customer.Dob = DateTime.Parse(dob);
                Customer.TypeOfCustomer = bool.Parse(type);
                Customer.TaxCode = tax;
                Customer.Active = true;

                _context.Customers.Add(Customer);
                int resultAddCustomer = _context.SaveChanges();
                if (resultAddCustomer != 0)
                {
                    Order.CustomerId = Customer.CustomerId;
                }
            }

            Order.ShipperId = int.Parse(shipperId);
            Order.UserName = employee.UserName;
            Order.OrderDate = DateTime.Now;
            Order.Active = true;
            Order.OderState = "notDelivery";
            Order.Total = decimal.Parse(total);
            if (!string.IsNullOrEmpty(payed))
            {
                Order.Deposit = decimal.Parse(payed);
            }
            Order.Cod = cod;
            Order.EstimatedDate = DateTime.Parse(estDate);
            Order.Region = region;

            _context.Oders.Add(Order);
            int result = _context.SaveChanges();
            if (result != 0)
            {
                foreach (var orderDetail in orderDetails)
                {
                    // Thiết lập giá trị BillId cho bản ghi BillDetail
                    orderDetail.OderId = Order.OderId;
                }

                // Thêm bản ghi BillDetail vào context
                _context.OderDetails.AddRange(orderDetails);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo vận đơn thành công!";
            }


            return RedirectToPage("./Index");
        }
    }
}
