using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Bills
{
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        private readonly Author _author;

        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }
        [BindProperty(SupportsGet = true)]
        public string message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string successMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string textSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime dateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime dateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string status { get; set; } = "all";

        private string raw_number, raw_textSearch, raw_dateFrom, raw_dateTo;

        public IList<Bill> Bills { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(2))
            {
                return RedirectToPage("/accessDenied", new { message = "Thu Ngân" });
            }
            //get data from form
            raw_number = HttpContext.Request.Query["pageSize"];
            if (!string.IsNullOrEmpty(raw_number))
            {
                pageSize = int.Parse(raw_number);
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

            raw_dateFrom = HttpContext.Request.Query["from"];
            raw_dateTo = HttpContext.Request.Query["to"];

            if (!string.IsNullOrEmpty(raw_dateFrom))
            {
                dateFrom = DateTime.Parse(raw_dateFrom);
                dateTo = DateTime.Parse(raw_dateTo);
            }

            //get bill list 
            var raw_filterForTotalCount = _context.Bills
                             .Include(b => b.Customer)
                             .Where(b => (b.BillId.ToString().Contains(textSearch)
                                 || b.Customer.CustomerName.Contains(textSearch)))
                             .Where(b => b.Active == true);

            if (status != "all")
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(b => (b.BillStatus.Contains(status)));
            }

            if (!string.IsNullOrEmpty(raw_dateFrom))
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(b => (b.Date >= dateFrom && b.Date <= dateTo));
            }

            TotalCount = raw_filterForTotalCount.Count();

            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }
            message = TempData["message"] as string;
            successMessage = TempData["successMessage"] as string;
            if (_context.Bills != null)
            {
                Bills = _dataService.GetBillsPagingAndFilter(pageIndex, pageSize, textSearch, status, raw_dateFrom, dateFrom, dateTo);
            }

            return Page();
        }
    }
}
