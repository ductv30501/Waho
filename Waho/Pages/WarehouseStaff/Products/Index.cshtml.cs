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
        public IList<Product> Products { get; set; } = default!;
        [BindProperty]
        public Product product { get; set; }

        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string textSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<SubCategory> subCategories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int subCategoryID { get; set; } = -1;

        private string raw_number, raw_subCategorySearch,raw_textSearch;

        [BindProperty(SupportsGet = true)]
        public List<Supplier> suppliers { get; set; }

        public async Task OnGetAsync()
        {
            //get data from form
            raw_number = HttpContext.Request.Query["pageSize"];

            if (!string.IsNullOrEmpty(raw_number))
            {
                pageSize = int.Parse(raw_number);
            }

            raw_subCategorySearch = HttpContext.Request.Query["subCategory"];

            if (!string.IsNullOrEmpty(raw_subCategorySearch))
            {
                subCategoryID = int.Parse(raw_subCategorySearch);
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
            //get subCategoris list by category
            subCategories =  _dataService.GetSubCategories(1);

            //get product list 
            TotalCount = _context.Products
                            .Where(p => (p.ProductName.Contains(textSearch)
                                || p.Trademark.Contains(textSearch)
                                || p.Supplier.Branch.Contains(textSearch)
                                || p.SubCategory.SubCategoryName.Contains(textSearch))
                                && (p.SubCategoryId == subCategoryID || subCategoryID == -1))
                            .Where(p => p.Active == true)
                            .Count();
            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if((pageIndex - 1)  > (TotalCount / pageSize)){
                pageIndex = 1;
            }

            if (_context.Products != null)
            {
                // categoryid = 1 
                Products = _dataService.GetProductsPagingAndFilter(pageIndex,pageSize,textSearch,subCategoryID,1) ;
            }

            suppliers = await _context.Suppliers.ToListAsync();
        }
        
    }
}
