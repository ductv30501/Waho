using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Waho.DataService;
using Waho.WahoModels;


namespace Waho.Pages.Cashier.Bills
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

        [BindProperty]
        public Bill Bill { get; set; } = default!;

        [BindProperty]
        public List<BillDetail> billDetails { get; set; } = default!;

        private Employee employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }

            Bill = await _context.Bills.Include(b => b.UserNameNavigation).Include(b => b.Customer).FirstOrDefaultAsync(m => m.BillId == id);
            billDetails = await _context.BillDetails.Include(bd => bd.Product).Where(bd => bd.BillId == id).ToListAsync();
            var lbillProducts = await _context.BillDetails.Include(bd => bd.Product).Where(bd => bd.BillId == id).Select(x => new
            {
                productId = x.ProductId,
                quantity = x.Quantity,
                discount = x.Discount,
                unitPrice = x.Product.UnitPrice,
            }).ToListAsync();

            ViewData["billProducts"] = lbillProducts;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            string customerId = HttpContext.Request.Form["customerId"];
            string billId = HttpContext.Request.Form["billId"];
            string total = HttpContext.Request.Form["total"];
            string listBillDetail = HttpContext.Request.Form["listBillDetail"];
            List<BillDetail> billDetails = JsonConvert.DeserializeObject<List<BillDetail>>(listBillDetail);

            Bill.CustomerId = int.Parse(customerId);
            Bill.Total = decimal.Parse(total.ToString());

            try
            {
                _context.Attach(Bill).State = EntityState.Modified;

                foreach (var billDetail in billDetails)
                {
                    // Thiết lập giá trị BillId cho bản ghi BillDetail
                    billDetail.BillId = Bill.BillId;
                    _context.BillDetails.Update(billDetail);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sửa hoá đơn thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(Bill.BillId))
                {
                    return NotFound();
                }
                else
                {
                    TempData["ErrorMessage"] = "Sửa hoá đơn thất bại!";
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.BillId == id);
        }
    }
}
