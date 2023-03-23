using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.InventorySheetManager
{
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        private readonly Author _author;
        //message
        public string message { get; set; }
        public string successMessage { get; set; }
        // paging
        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int TotalCount { get; set; } = 0;

        private string raw_pageSize;
        public DetailsModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }

        public InventorySheet _inventorySheet { get; set; }
        public List<InventorySheetDetail> inventorySheetDetails { get; set; }
        public List<InventorySheetDetail> inventorySheetDetailAll { get; set; }
        [BindProperty(SupportsGet = true)]
        public int _inventorySheetID { get; set; }
        public async Task<IActionResult> OnGetAsync(int inventorySheetID)
        {
            //author
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            }
            //get data from form
            raw_pageSize = HttpContext.Request.Query["pageSize"];
            if (inventorySheetID != 0)
            {
                _inventorySheetID = inventorySheetID;
            }
            if (HttpContext.Request.HasFormContentType == true)
            {

                if (!string.IsNullOrEmpty(HttpContext.Request.Form["inventorySheetID"]))
                {
                    _inventorySheetID = Int32.Parse(HttpContext.Request.Form["inventorySheetID"]);
                }
            }
            if (!string.IsNullOrEmpty(raw_pageSize))
            {
                pageSize = int.Parse(raw_pageSize);
            }

            if (inventorySheetID == null || _context.InventorySheetDetails == null)
            {
                //message
                message = "Không tìm thấy mã phiếu kiểm kho";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            _inventorySheet = _context.InventorySheets
                                    .Include(i => i.UserNameNavigation)
                                    .FirstOrDefault(i => i.InventorySheetId == inventorySheetID);
            var _inventorySheetDetails = await _context.InventorySheetDetails
                                                        .Include(i => i.InventorySheet)
                                                        .Include(i => i.Product)
                                                        .Include(i => i.InventorySheet.UserNameNavigation)
                                                        .Where(i => i.InventorySheetId == _inventorySheetID)
                                                        .ToListAsync();
            inventorySheetDetailAll = _inventorySheetDetails.ToList();
            TotalCount = _inventorySheetDetails.Count;
            if (_inventorySheetDetails != null)
            {
                inventorySheetDetails = _dataService.getInventorySheetDetailPaging(pageIndex, pageSize, inventorySheetID);
                return Page();
            }
            //message
            message = "Không tìm thấy mã phiếu kiểm kho";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("debug");
            InventorySheet _inventorySheetUpdate = new InventorySheet();
            var req = HttpContext.Request;
            //get data form form submit 
            string raw_EmployeeID = req.Form["employeeID"];
            string raw_inventorySheetID = req.Form["inventorySheetID"];
            string raw_date = req.Form["date"];
            string raw_description = req.Form["description"];
            _inventorySheetUpdate.InventorySheetId = Int32.Parse(raw_inventorySheetID);
            _inventorySheetUpdate.UserName = raw_EmployeeID;
            if (string.IsNullOrWhiteSpace(raw_date))
            {
                //message
                message = "Ngày kiểm kho không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            _inventorySheetUpdate.Date = DateTime.Parse(raw_date);
            _inventorySheetUpdate.Description = raw_description;
            _inventorySheetUpdate.Active = true;
            _context.Attach(_inventorySheetUpdate).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                //success message
                successMessage = "Chỉnh sửa thông tin phiếu thành công";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventorySheetDetailExists(_inventorySheetUpdate.InventorySheetId))
                {
                    //message
                    message = "Không tìm thấy phiếu kiểm kho nào phù hợp";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
                else
                {
                    throw;
                }

            }
        }
        public async Task<IActionResult> OnPostUpdateDetail()
        {
            var req = HttpContext.Request;
            //get data form form submit 
            string currennumbers = req.Form["CurNwareHouse"];
            int inventorySheetID =Int32.Parse(req.Form["inventorySheetID"]);
            string productIDs = req.Form["productIDUpdate"];
            string productQuantity = req.Form["Quantity"];
            string[] currennumbersList = currennumbers.Split(',');
            string[] productIDsList = productIDs.Split(',');
            string[] productQuantityList = productQuantity.Split(',');
            int count = 0;
            foreach (var i in currennumbersList)
            {
                int currentNumber = Int32.Parse(i);
                int productID = Int32.Parse(productIDsList[count]);
                int quantity = Int32.Parse(productQuantityList[count]);
                // update product information in inventorySheet
                InventorySheetDetail _inventorySheetDetail = new InventorySheetDetail();
                _inventorySheetDetail = await _context.InventorySheetDetails.FirstOrDefaultAsync(p => p.ProductId == productID && p.InventorySheetId == inventorySheetID);
                _inventorySheetDetail.CurNwareHouse = currentNumber;
                _context.Attach(_inventorySheetDetail).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // update information of product in product list
                Product product = new Product();
                product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productID);
                product.Quantity = quantity;
                _context.Attach(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //message
                successMessage = "lưu thông tin sản phẩm thành công";
                TempData["successMessage"] = successMessage;
                count++;
            }

            try
            {
                //success message
                successMessage = "Lưu thông tin sản phẩm kiểm tra thành công";
                TempData["SuccessMessage"] = successMessage;
                return RedirectToPage("./Details", new { inventorySheetID });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Page();
            }
        }
        private bool InventorySheetDetailExists(int id)
        {
            return _context.InventorySheets.Any(e => e.InventorySheetId == id);
        }
    }
}
