using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.Orders
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
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public string textSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string dateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public string dateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string estDateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public string estDateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string status { get; set; } = "all";

        [BindProperty(SupportsGet = true)]
        public string active { get; set; } = "all";

        private string raw_number, raw_textSearch;

        public IList<Oder> Orders { get; set; } = default!;

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

            dateFrom = HttpContext.Request.Query["dateFrom"];
            dateTo = HttpContext.Request.Query["dateTo"];
            estDateFrom = HttpContext.Request.Query["estDateFrom"];
            estDateTo = HttpContext.Request.Query["estDateTo"];


            //get bill list 
            var raw_filterForTotalCount = _context.Oders
                             .Include(o => o.Customer)
                             .Include(o=>o.Shipper)
                             .Where(o => (o.OderId.ToString().Contains(textSearch)
                                 || o.Shipper.ShipperName.Contains(textSearch)
                                 || o.Customer.CustomerName.Contains(textSearch)));

            if (active != "all")
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(o => (o.Active.ToString().Contains(active)));
            }

            if (status != "all")
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(o => (o.OderState.Contains(status)));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(o => (o.OrderDate >= DateTime.Parse(dateFrom) && o.OrderDate <= DateTime.Parse(dateTo)));
            }

            if (!string.IsNullOrEmpty(estDateTo))
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(o => (o.EstimatedDate >= DateTime.Parse(estDateFrom) && o.EstimatedDate <= DateTime.Parse(estDateTo)));
            }

            TotalCount = raw_filterForTotalCount.Count();

            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }

            if (_context.Oders != null)
            {
                Orders = _dataService.GetOrdersPagingAndFilter(pageIndex, pageSize, textSearch, status, dateFrom, estDateFrom, estDateTo, dateTo, active);
            }

            return Page();
        }
    }
}
