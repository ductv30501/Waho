using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Waho.DataService;
using Waho.WahoModels;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace Waho.Pages.Cashier.ReturnOrders
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
        [BindProperty(SupportsGet = true)]
        public List<Product> products { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Customer> customers { get; set; }
        private Employee employee { get; set; }
        [BindProperty(SupportsGet = true)]
        public int billCategory { get; set; }

        [BindProperty]
        public ReturnOrder _ReturnOrder { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            string customerId = HttpContext.Request.Form["customerId"];
            string total = HttpContext.Request.Form["total"];
            string idBill = HttpContext.Request.Form["idBill"];
            string state = HttpContext.Request.Form["state"];
            string paidCustomer = HttpContext.Request.Form["paidCustomer"];
            string description = HttpContext.Request.Form["description"];
            string listBillDetail = HttpContext.Request.Form["listBillDetail"];
            List<ReturnOrderProduct> billDetails = JsonConvert.DeserializeObject<List<ReturnOrderProduct>>(listBillDetail);

            var employeeJson = HttpContext.Session.GetString("Employee");

            if (employeeJson != null)
            {
                employee = JsonSerializer.Deserialize<Employee>(employeeJson);
            }
            //validate number of  product
            if (billCategory == 1)
            {
                //get details in the bill of customer
                List<BillDetail> _billDetails = _context.BillDetails.Where(b => b.BillId == Int32.Parse(idBill)).ToList();
                Boolean check = false;
                foreach (var item in _billDetails)
                {
                    // the details return
                    foreach (var itemReturn in billDetails)
                    {
                        if (item.ProductId == itemReturn.ProductId)
                        {
                            if (itemReturn.Quantity > item.Quantity)
                            {
                                //message error
                                TempData["ErrorMessage"] = "Số lượng trả hàng không được vượt quá số lượng đã mua!";
                                return Page();
                            }
                            if (item.Discount != itemReturn.Discount)
                            {
                                //message error
                                TempData["ErrorMessage"] = "Khác giá trị giảm giá!";
                                return Page();
                            }
                            check = true;
                        }
                    }
                }
                if (!check)
                {
                    //message error
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm trong hóa đơn!";
                    return Page();
                }
            }
            else
            {
                List<OderDetail> _OderDetail = _context.OderDetails.Where(b => b.OderId == Int32.Parse(idBill)).ToList();
                Boolean check = false;
                foreach (var item in _OderDetail)
                {
                    foreach (var itemReturn in billDetails)
                    {
                        if (item.ProductId == itemReturn.ProductId)
                        {
                            if (itemReturn.Quantity > item.Quantity)
                            {
                                //message error
                                TempData["ErrorMessage"] = "Số lượng trả hàng không được vượt quá số lượng đã mua!";
                                return Page();
                            }
                            if (item.Discount != itemReturn.Discount)
                            {
                                //message error
                                TempData["ErrorMessage"] = "Khác giá trị giảm giá!";
                                return Page();
                            }
                            check = true;
                        }
                    }
                }
                if (!check)
                {
                    //message error
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm trong đơn vận!";
                    return Page();
                }
            }
            // validate product and bill id already in return order
            // return order have billID = idBill
            ReturnOrder _returnorderCheck = _context.ReturnOrders.FirstOrDefault(r => r.BillId == Int32.Parse(idBill));
            if (_returnorderCheck != null)
            {
                // list return product of the bill have billID = idbill
                List<ReturnOrderProduct> returnOrderProducts = _context.ReturnOrderProducts
                                                                .Include(r => r.Product)
                                                                .Where(r => r.ReturnOrderId == _returnorderCheck.ReturnOrderId)
                                                                .ToList();
                //  billdetail : list product return 
                foreach(var billDetail in billDetails)
                {
                    foreach (var r in returnOrderProducts)
                    {
                        if (billDetail.ProductId == r.ProductId)
                        {
                            if (billCategory == 1)
                            {   
                                // detail of the product of the bill bought
                                var detailBill = _context.BillDetails.Where(r => r.ProductId== r.ProductId)
                                                                        .Where(r => r.BillId== Int32.Parse(idBill))
                                                                        .FirstOrDefault();
                                if((detailBill.Quantity - r.Quantity) <= 0)
                                {
                                    //message error
                                    TempData["ErrorMessage"] = "Sản phẩm " + r.Product.ProductName + "đã được hoàn hết số lượng trong bill!" +
                                                                "Số lượng có thể trả"+ (detailBill.Quantity - r.Quantity);
                                    return Page();
                                }
                            }
                            else
                            {
                                // detail of the product of the ordered
                                var detailBill = _context.OderDetails.Where(r => r.ProductId == r.ProductId)
                                                                        .Where(r => r.OderId == Int32.Parse(idBill))
                                                                        .FirstOrDefault();
                                if ((detailBill.Quantity - r.Quantity) <= 0)
                                {
                                    //message error
                                    TempData["ErrorMessage"] = "Sản phẩm " + r.Product.ProductName + "đã được hoàn hết số lượng trong order!" +
                                                                "Số lượng có thể trả" + (detailBill.Quantity - r.Quantity);
                                    return Page();
                                }
                            }
                        }
                    }
                }
            }
            //add information for return order
            _ReturnOrder.UserName = employee.UserName;
            _ReturnOrder.CustomerId = int.Parse(customerId);
            _ReturnOrder.Date = DateTime.Now;
            _ReturnOrder.Active = true;
            _ReturnOrder.PayCustomer = decimal.Parse(total);
            _ReturnOrder.PaidCustomer = decimal.Parse(paidCustomer);
            _ReturnOrder.Description = description;
            _ReturnOrder.BillId = Int32.Parse(idBill);
            if (string.IsNullOrEmpty(state))
            {
                _ReturnOrder.State = false;
            }
            _ReturnOrder.State = true;
            _context.ReturnOrders.Add(_ReturnOrder);
            int result = _context.SaveChanges();
            if (result != 0)
            {
                foreach (var billDetail in billDetails)
                {
                    // Thiết lập giá trị BillId cho bản ghi BillDetail
                    billDetail.ReturnOrderId = _ReturnOrder.ReturnOrderId;
                }

                // Thêm bản ghi BillDetail vào context
                _context.ReturnOrderProducts.AddRange(billDetails);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo hoàn đơn thành công!";
            }
            return RedirectToPage("./Index");
        }
    }
}
