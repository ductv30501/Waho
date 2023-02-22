using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
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
        public IList<Product> Product { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string textSearch { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public List<SubCategory> subCategories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int subCategoryID { get; set; } = -1;
        public async Task OnGetAsync()
        {
            string raw_number = HttpContext.Request.Query["pageSize"];
            if (!string.IsNullOrEmpty(raw_number))
            {
                pageSize = int.Parse(raw_number);
            }
            string raw_subCategorySearch = HttpContext.Request.Query["subCategory"];

            if (!string.IsNullOrEmpty(raw_subCategorySearch))
            {
                subCategoryID = int.Parse(raw_subCategorySearch);
            }
            textSearch = HttpContext.Request.Query["textSearch"];
            //get subCategoris list
            subCategories =  _dataService.GetSubCategories(1);

            //get product list default
            TotalCount = _context.Products.Count();
            if (_context.Products != null)
            {
                Product = _dataService.GetProductsPagingAndFilter(pageIndex,pageSize,textSearch,subCategoryID,1) ;
            }
        }
        
    }
}
