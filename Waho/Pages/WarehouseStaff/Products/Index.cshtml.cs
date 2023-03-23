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

        private string raw_number, raw_subCategorySearch, raw_textSearch;

        [BindProperty(SupportsGet = true)]
        public List<Supplier> suppliers { get; set; }
        //filter by location
        public List<string> locations { get; set; }
        [BindProperty(SupportsGet = true)]
        public string location { get; set; } = "all";
        //filter price range 
        [BindProperty(SupportsGet = true)]
        public int priceTo { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public int priceFrom { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public string inventoryLevel { get; set; } = "all";
        public string supplierName { get; set; } = "all";

        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            }
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
                textSearch = raw_textSearch.Trim();
            }
            else
            {
                textSearch = "";
            }
            // location
            string raw_location = HttpContext.Request.Query["location"];
            if (!string.IsNullOrWhiteSpace(raw_location))
            {
                location = raw_location.Trim();
            }
            else
            {
                location = "all";
            }
            // price range
            string raw_priceFrom = HttpContext.Request.Query["priceFrom"];
            if (!string.IsNullOrWhiteSpace(raw_priceFrom))
            {
                priceFrom = int.Parse(raw_priceFrom);
            }
            else
            {
                priceFrom = 0;
            }
            string raw_priceTo = HttpContext.Request.Query["priceTo"];
            if (!string.IsNullOrWhiteSpace(raw_priceTo))
            {
                priceFrom = int.Parse(raw_priceTo);
            }
            else
            {
                priceFrom = 0;
            }
            // inventory level
            string raw_inventoryLevel = HttpContext.Request.Query["inventoryLevel"];
            if (!string.IsNullOrWhiteSpace(raw_inventoryLevel))
            {
                inventoryLevel = raw_inventoryLevel;
            }
            else
            {
                inventoryLevel = "all";
            }
            // supplier
            string raw_supplier= HttpContext.Request.Query["supplierName"];
            if (!string.IsNullOrWhiteSpace(raw_supplier))
            {
                supplierName = raw_supplier;
            }
            else
            {
                supplierName = "all";
            }
            //get subCategoris list by category
            subCategories = _dataService.GetSubCategories(1);
            //get location list 
            locations = _context.Products.Select(s => s.Location).Distinct().ToList();
            //get product list 
            var query = _context.Products
                            .Where(p => (p.ProductName.ToLower().Contains(textSearch.ToLower())
                                || p.Trademark.ToLower().Contains(textSearch.ToLower())
                                || p.Supplier.Branch.ToLower().Contains(textSearch.ToLower())
                                || p.SubCategory.SubCategoryName.ToLower().Contains(textSearch.ToLower()))
                                && (p.SubCategoryId == subCategoryID || subCategoryID == -1))
                            .Where(p => p.Active == true);
            // filter location
            if (location != "all")
            {
                query = query.Where(p => p.Location == location);
            }
            // filter price range
            if (priceTo > 0)
            {
                if (priceFrom >0)
                {
                    query = query.Where(p => p.UnitPrice >= priceFrom && p.UnitPrice <= priceTo);
                }
                else
                {
                    query = query.Where(p => p.UnitPrice <= priceTo);
                }
            }
            if (priceFrom > 0)
            {
                query = query.Where(p => p.UnitPrice >= priceFrom);
            }
            // filter inventory level
            if (inventoryLevel != "all")
            {
                if (inventoryLevel == "min")
                {
                    query = query.Where(p => p.Quantity < p.InventoryLevelMin );
                }
                else
                {
                    query = query.Where(p => p.Quantity > p.InventoryLevelMax);
                }
            }
            // supplier 
            if (supplierName != "all")
            {
                query = query.Where(p => p.SupplierId == int.Parse(supplierName));
            }
            TotalCount = query.Count();
            //gán lại giá trị pageIndex khi page index vợt quá pageSize khi filter
            if ((pageIndex - 1) > (TotalCount / pageSize))
            {
                pageIndex = 1;
            }
            message = TempData["message"] as string;
            successMessage = TempData["successMessage"] as string;
            if (_context.Products != null)
            {
                // categoryid = 1 
                Products = _dataService.GetProductsPagingAndFilter(pageIndex, pageSize, textSearch, subCategoryID, 1,location,priceFrom,priceTo, inventoryLevel);
            }
            //get message
            suppliers = await _context.Suppliers.Where(s => s.Active == true).ToListAsync();
            return Page();
        }

    }
}
