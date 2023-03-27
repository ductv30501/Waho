using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Employees
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
        [BindProperty(SupportsGet = true)]
        public string status { get; set; } = "all";
        private string raw_pageSize, raw_textSearch;
        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }
        [BindProperty(SupportsGet = true)]
        public IList<Employee> Employee { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string title { get; set; } = "all";

        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            //get data from form
            raw_pageSize = HttpContext.Request.Query["pageSize"];
            if (!string.IsNullOrEmpty(raw_pageSize))
            {
                pageSize = int.Parse(raw_pageSize);
            }
            raw_textSearch = HttpContext.Request.Query["textSearch"];
            if (!string.IsNullOrWhiteSpace(raw_textSearch))
            {
                textSearch = raw_textSearch.Trim().ToLower();
            }
            else
            {
                textSearch = "";
            }
            string raw_title = HttpContext.Request.Query["title"];
            if (!string.IsNullOrWhiteSpace(raw_title))
            {
                title = raw_title.Trim();
            }
            else
            {
                title = "all";
            }
            var query = _context.Employees
                            .Where(e => e.EmployeeName.ToLower().Contains(textSearch) || e.Email.ToLower().Contains(textSearch)
                                    || e.Dob.ToString().ToLower().Contains(textSearch) || e.Title.ToLower().Contains(textSearch)
                                    || e.Phone.ToLower().Contains(textSearch) || e.Region.ToLower().Contains(textSearch)
                                    || e.Address.ToLower().Contains(textSearch) || e.HireDate.ToString().ToLower().Contains(textSearch)
                                    || e.Role.ToString().Contains(textSearch));
            if (status != "all")
            {
                query = query.Where(c => (c.Active.ToString().Contains(status)));
            }
            if (title != "all")
            {
                query = query.Where(e => e.Role == int.Parse(title));
            }
            TotalCount = query.Count();
            message = TempData["message"] as string;
            successMessage = TempData["successMessage"] as string;
            if (_context.Employees != null)
            {
                Employee = _dataService.getEmployeePaging(pageIndex, pageSize, textSearch, title,status);
            }
            return Page();
        }
    }
}
