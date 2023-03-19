using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.InventorySheetManager
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
        public string textSearch { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public string employeeID { get; set; } = "";
        private string raw_pageSize, raw_EmployeeSearch, raw_textSearch;
        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }
        //list employee
        [BindProperty(SupportsGet = true)]
        public List<Employee> employees { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<InventorySheet> InventorySheetList { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
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
                employeeID = raw_EmployeeSearch.Trim().ToLower();
            }
            else
            {
                employeeID = "";
            }
            raw_textSearch = HttpContext.Request.Query["textSearch"];
            if (!string.IsNullOrWhiteSpace(raw_textSearch))
            {
                textSearch = raw_textSearch;
            }
            else
            {
                textSearch = "";
            }

            // get list WareHouse Employee
            employees = await _context.Employees.Where(e => e.Role != 2).ToListAsync();
            //get inventory sheet list 
            TotalCount = _context.InventorySheets.Include(p => p.UserNameNavigation)
                            .Where(i => i.Active == true)
                            .Where(i => i.UserNameNavigation.EmployeeName.ToLower().Contains(textSearch)
                                    || i.Description.ToLower().Contains(textSearch))
                            .Where(i => i.UserName == employeeID || employeeID == "")
                            .Count();
            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }
            message = TempData["message"] as string;
            successMessage = TempData["successMessage"] as string;
            if (_context.InventorySheets != null)
            {
                InventorySheetList = _dataService.getInventoryPagingAndFilter(pageIndex, pageSize, textSearch, employeeID);
            }
            return Page();
        }
    }
}
