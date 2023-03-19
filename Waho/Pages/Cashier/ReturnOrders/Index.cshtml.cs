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
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        private readonly Author _author;
        //message
        [BindProperty(SupportsGet = true)]
        public string message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string successMessage { get; set; }
        // paging
        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string textSearch { get; set; }
        public string employeeID { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public DateTime dateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime dateTo { get; set; }
        private string raw_pageSize, raw_textSearch, raw_EmployeeSearch, raw_status, raw_dateFrom, raw_dateTo;
        //list employee
        public List<Employee> employees { get; set; }
        public string status { get; set; }
        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }

        public IList<ReturnOrder> ReturnOrder { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }
            //get data from form
            raw_pageSize = HttpContext.Request.Query["pageSize"];
            if (!string.IsNullOrEmpty(raw_pageSize))
            {
                pageSize = int.Parse(raw_pageSize);
            }
            raw_EmployeeSearch = HttpContext.Request.Query["employeeID"];
            if (!string.IsNullOrEmpty(raw_EmployeeSearch))
            {
                employeeID = raw_EmployeeSearch;
            }
            else
            {
                employeeID = "";
            }
            raw_textSearch = HttpContext.Request.Query["textSearch"];
            if (!string.IsNullOrWhiteSpace(raw_textSearch))
            {
                textSearch = raw_textSearch.Trim();
            }
            else
            {
                textSearch = "";
            }
            raw_status = HttpContext.Request.Query["status"];
            if (!string.IsNullOrWhiteSpace(raw_status))
            {
                status = raw_status;
            }
            else
            {
                status = "";
            }
            Boolean _status = status == "true" ? true : false;
            raw_dateFrom = HttpContext.Request.Query["dateFrom"];
            raw_dateTo = HttpContext.Request.Query["dateTo"];

            if (!string.IsNullOrEmpty(raw_dateFrom))
            {
                dateFrom = DateTime.Parse(raw_dateFrom);
            }
            else
            {
                raw_dateFrom = "";
            }
            if (!string.IsNullOrEmpty(raw_dateTo))
            {
                dateTo = DateTime.Parse(raw_dateTo);
            }
            else
            {
                raw_dateTo = "";
            }
            // get list WareHouse Employee
            employees = await _context.Employees.Where(e => e.Role != 3).ToListAsync();
            var query = _context.ReturnOrders.Include(p => p.UserNameNavigation)
                           .Include(i => i.Customer)
                           .Where(i => i.Active == true)
                           .Where(i => i.UserNameNavigation.EmployeeName.ToLower().Contains(textSearch.ToLower())
                                   || i.Description.ToLower().Contains(textSearch.ToLower())
                                   || i.Customer.CustomerName.ToLower().Contains(textSearch.ToLower()))
                           .Where(i => i.UserName == employeeID || employeeID == "");
            // check status to filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.State == _status);
            }
            // compare date to filter
            if (!string.IsNullOrEmpty(raw_dateFrom) && !string.IsNullOrEmpty(raw_dateTo))
            {
                query = query.Where(i => i.Date >= dateFrom && i.Date <= dateTo);
            }
            TotalCount = query.Count();
            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }
            message = TempData["message"] as string;
            successMessage = TempData["successMessage"] as string;
            if (_context.ReturnOrders != null)
            {
                ReturnOrder = _dataService.getreturnOrderPagingAndFilter(pageIndex, pageSize, textSearch, employeeID,status, raw_dateFrom, raw_dateTo);
            }
            return Page();
        }
    }
}
