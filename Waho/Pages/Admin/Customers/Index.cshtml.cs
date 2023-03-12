using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Customers
{
    public class IndexModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;

        public IndexModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService)
        {
            _context = context;
            _dataService = dataService;
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
        public string status { get; set; } = "all";

        [BindProperty(SupportsGet = true)]
        public string typeCustomer { get; set; } = "all";

        [BindProperty(SupportsGet = true)]
        public string dateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public string dateTo { get; set; }

        private string raw_number, raw_textSearch;

        public IList<Customer> Customers { get;set; } = default!;

        public async Task OnGetAsync()
        {
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

            //get customer list 
            var raw_filterForTotalCount = _context.Customers
                             .Where(c => (c.CustomerName.Contains(textSearch)
                                 || c.Phone.Contains(textSearch)
                                 || c.Email.Contains(textSearch)
                                 || c.TaxCode.Contains(textSearch)));
            
            if (status != "all")
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(c => (c.Active.ToString().Contains(status)));
            }

            if (typeCustomer != "all")
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(c => (c.TypeOfCustomer.ToString().Contains(typeCustomer)));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                raw_filterForTotalCount = raw_filterForTotalCount.Where(c => (c.Dob >= DateTime.Parse(dateFrom) && c.Dob <= DateTime.Parse(dateTo)));
            }

            TotalCount = raw_filterForTotalCount.Count();

            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }

            if (_context.Customers != null)
            {
                Customers =  _dataService.GetCustomersPagingAndFilter(pageIndex, pageSize, textSearch, status, dateFrom, dateTo, typeCustomer);
            }
        }
    }
}
