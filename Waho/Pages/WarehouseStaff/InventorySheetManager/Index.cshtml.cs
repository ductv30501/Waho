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
        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService)
        {
            _context = context;
            _dataService = dataService;
        }
        //list employee
        [BindProperty(SupportsGet = true)]
        public List<Employee> employees { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<InventorySheet> InventorySheetList { get;set; }

        public async Task OnGetAsync()
        {
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
                textSearch = raw_textSearch;
            }
            else
            {
                textSearch = "";
            }

            // get list WareHouse Employee
            employees = await _context.Employees.ToListAsync();
            //get inventory sheet list 
            TotalCount = _context.InventorySheets
                            //.Where(i => (i.UserNameNavigation.EmployeeName.Contains(textSearch)
                            //        || i.Description.Contains(textSearch)) && (i.UserName == employeeID || i.UserName == ""))
                            .Include(p => p.UserNameNavigation)
                            .Count();
            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }

            if (_context.InventorySheets != null)
            {
                InventorySheetList = _dataService.getInventoryPagingAndFilter(pageIndex, pageSize, textSearch, employeeID);
            }
        }
    }
}
