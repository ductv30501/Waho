using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.ReturnOrders
{
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        //message
        public string message { get; set; }
        public string successMessage { get; set; }
        // paging
        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        private string raw_pageSize;
        [BindProperty(SupportsGet = true)]
        public int _returnOrderID { get; set; }
        public DetailsModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        public ReturnOrder ReturnOrder { get; set; }
        public List<ReturnOrderProduct> returnOrderProducts { get; set; }
        public async Task<IActionResult> OnGetAsync(int returnOrderID)
        {
            // get data from form
            raw_pageSize = HttpContext.Request.Query["pageSize"];
            if (returnOrderID != 0)
            {
                _returnOrderID = returnOrderID;
            }
            if (HttpContext.Request.HasFormContentType == true)
            {

                if (!string.IsNullOrEmpty(HttpContext.Request.Form["returnOrderID"]))
                {
                    _returnOrderID = Int32.Parse(HttpContext.Request.Form["returnOrderID"]);
                }
            }

            if (!string.IsNullOrEmpty(raw_pageSize))
            {
                pageSize = int.Parse(raw_pageSize);
            }
            // get return order
            if (returnOrderID == null || _context.ReturnOrderProducts == null)
            {
                //message
                message = "Không tìm thấy mã hoàn đơn";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            var returnorder = await _context.ReturnOrders
                                            .Include(r => r.Customer)
                                            .Include(r => r.UserNameNavigation)
                                            .FirstOrDefaultAsync(m => m.ReturnOrderId == _returnOrderID);
            if (returnorder == null)
            {
                //message
                message = "Không tìm thấy hoàn đơn tương ứng returnorder";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            else
            {
                ReturnOrder = returnorder;
            }
            // get list product of return order and paging
            TotalCount = _context.ReturnOrderProducts.Where(r => r.ReturnOrderId == _returnOrderID).Count();

            if (_context.ReturnOrderProducts != null)
            {
                returnOrderProducts = _dataService.getReturnOrderProductPaging(pageIndex, pageSize, _returnOrderID);
                return Page();
            }
            //message
            message = "Không tìm thấy hoàn đơn tương ứng";
            //TempData["message"] = message;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ReturnOrder _returnOrderUpdate = new ReturnOrder();
            //get data form form submit 
            var req = HttpContext.Request;
            string raw_EmployeeID = req.Form["employeeID"];
            string raw_returnOrderID = req.Form["returnOrderID"];
            string raw_customerID = req.Form["customerID"];
            string raw_date = req.Form["date"];
            string raw_state = req.Form["state"];
            string raw_description = req.Form["description"];
            string raw_payCustomer = req.Form["payCustomer"];
            string raw_paidCustomer = req.Form["paidCustomer"];
            _returnOrderUpdate.ReturnOrderId = Int32.Parse(raw_returnOrderID);
            _returnOrderUpdate.UserName = raw_EmployeeID;
            _returnOrderUpdate.CustomerId = Int32.Parse(raw_customerID);
            if (string.IsNullOrWhiteSpace(raw_date))
            {
                //message
                message = "Ngày tạo đơn không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            _returnOrderUpdate.Date = DateTime.Parse(raw_date);
            _returnOrderUpdate.Description = raw_description;
            _returnOrderUpdate.Active = true;
            if (string.IsNullOrWhiteSpace(raw_paidCustomer))
            {
                //message
                message = "Số tiền đã trả khách không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            _returnOrderUpdate.PaidCustomer = Int32.Parse(raw_paidCustomer);
            if (string.IsNullOrWhiteSpace(raw_payCustomer))
            {
                //message
                message = "Số tiền cần trả khách không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            if (string.IsNullOrWhiteSpace(raw_state))
            {
                _returnOrderUpdate.State = false;
            }
            else
            {
                _returnOrderUpdate.State = true;
            }
            _returnOrderUpdate.PayCustomer = Int32.Parse(raw_payCustomer);
            _context.Attach(_returnOrderUpdate).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                //success message
                successMessage = "Chỉnh sửa thông tin phiếu thành công";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReturnOrderExists(_returnOrderUpdate.ReturnOrderId))
                {
                    //message
                    message = "Không tìm thấy hoàn đơn nào phù hợp";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
                else
                {
                    throw;
                }

            }

        }
        private bool ReturnOrderExists(int id)
        {
            return _context.ReturnOrders.Any(e => e.ReturnOrderId == id);
        }
    }
}
