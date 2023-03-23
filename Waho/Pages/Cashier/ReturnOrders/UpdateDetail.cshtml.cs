using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Waho.DataService;
using Waho.WahoModels;
using Microsoft.EntityFrameworkCore;

namespace Waho.Pages.Cashier.ReturnOrders
{
    public class UpdateDetailModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly DataServiceManager _dataService;
        private readonly Author _author;
        public UpdateDetailModel(Waho.WahoModels.WahoContext context, DataServiceManager dataService, Author author)
        {
            _context = context;
            _dataService = dataService;
            _author = author;
        }
        //message
        public string message { get; set; }
        public string successMessage { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //author
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            }
            return Page();
        }
        public async Task<IActionResult> OnPostUpdateDetail()
        {
            Console.WriteLine("đi vào đây");
            var req = HttpContext.Request;
            //get data form form submit 
            int inventorySheetID = Int32.Parse(req.Form["inventorySheetID"]);
            int productID = Int32.Parse(req.Form["productIDUpdate"]);

            // update product information in inventorySheet
            InventorySheetDetail _inventorySheetDetail = new InventorySheetDetail();
            _inventorySheetDetail = await _context.InventorySheetDetails.FirstOrDefaultAsync(p => p.ProductId == productID && p.InventorySheetId == inventorySheetID);
            _inventorySheetDetail.CurNwareHouse = Int32.Parse(req.Form["CurNwareHouse-" + productID]);
            _context.Attach(_inventorySheetDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // update information of product in product list
            Product product = new Product();
            product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productID);
            product.Quantity = Int32.Parse(req.Form["Quantity-" + productID]);
            _context.Attach(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            try
            {
                //success message
                successMessage = "Chỉnh sửa thông tin kiểm tra thành công";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./WarehouseStaff/InventorySheetManager/Details", new { inventorySheetID });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Page();
            }
        }
    }
}
