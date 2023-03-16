using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
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
        private string raw_pageSize, raw_textSearch;
        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }
        [BindProperty(SupportsGet = true)]
        public IList<Supplier> Supplier { get;set; } = default!;

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
            TotalCount = _context.Suppliers
                            .Where(s => s.Branch.ToLower().Contains(textSearch) || s.Address.ToLower().Contains(textSearch) || s.CompanyName.ToLower().Contains(textSearch) || s.Phone.ToLower().Contains(textSearch)
                            || s.City.ToLower().Contains(textSearch) || s.Region.ToLower().Contains(textSearch) || s.TaxCode.ToLower().Contains(textSearch))
                            .Where(s => s.Active == true)
                            .Count();
            message = TempData["message"] as string;
            successMessage = TempData["successMessage"] as string;
            if (_context.Suppliers != null)
            {
                Supplier = _dataService.GetSupplierPagingAndFilter(pageIndex, pageSize, textSearch);
            }

            return Page();
        }
    }
}
