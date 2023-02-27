using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
{
    public class EditModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public EditModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product =  await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
           ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "SubCategoryId", "SubCategoryId");
           ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var req = HttpContext.Request;
            //get data form form submit 
            string raw_productID = req.Form["productIDUpdate"];
            string raw_productName = req.Form["productNameUpdate"];
            string raw_importPrice = req.Form["importPriceUpdate"];
            string raw_unitInStock = req.Form["unitInStockUpdate"];
            string raw_unitPrice = req.Form["unitPriceUpdate"];
            string raw_trademark = req.Form["trademarkUpdate"];
            string raw_weight = req.Form["weightUpdate"];
            string raw_location = req.Form["locationUpdate"];
            string raw_unit = req.Form["unitUpdate"];
            string raw_inventoryLevelMin = req.Form["inventoryLevelMinUpdate"];
            string raw_inventoryLevelMax = req.Form["inventoryLevelMaxUpdate"];
            string raw_description = req.Form["descriptionUpdate"];
            string raw_subCategory = req.Form["subCategoryUpdate"];
            string raw_supplierID = req.Form["supplierIDUpdate"];

            Product.ProductId = Int32.Parse(raw_productID);
            Product.ProductName = raw_productName;
            if (string.IsNullOrWhiteSpace(raw_importPrice))
            {
                int import_Price = Int32.Parse(raw_importPrice);
                if (import_Price > 0)
                {
                    Product.ImportPrice = import_Price;
                }
                else
                {
                    // messagse
                }
            }
            if (string.IsNullOrWhiteSpace(raw_unitInStock))
            {
                int unitInStock = Int32.Parse(raw_unitInStock);
                if (unitInStock > 0)
                {
                    Product.UnitInStock = unitInStock;
                }
                else
                {
                    // messagse
                }
            }
            if (string.IsNullOrWhiteSpace(raw_unitPrice))
            {
                int unitPrice = Int32.Parse(raw_unitPrice);
                if (unitPrice > 0)
                {
                    Product.UnitPrice = unitPrice;
                }
                else
                {
                    // messagse
                }
            }
            Product.Trademark = raw_trademark;
            if (string.IsNullOrWhiteSpace(raw_weight))
            {
                int weight = Int32.Parse(raw_weight);
                if (weight > 0)
                {
                    Product.Weight = weight;
                }
                else
                {
                    // messagse
                }
            }
            Product.Location = raw_location;
            Product.Description = raw_description;
            Product.Unit = raw_unit;
            if (string.IsNullOrWhiteSpace(raw_inventoryLevelMin))
            {
                int inventoryLevelMin = Int32.Parse(raw_inventoryLevelMin);
                if (inventoryLevelMin > 0)
                {
                    Product.InventoryLevelMin = inventoryLevelMin;
                }
                else
                {
                    // messagse
                }
            }
            if (string.IsNullOrWhiteSpace(raw_inventoryLevelMax))
            {
                int inventoryLevelMax = Int32.Parse(raw_inventoryLevelMax);
                if (inventoryLevelMax > 0)
                {
                    Product.InventoryLevelMax = inventoryLevelMax;
                }
                else
                {
                    // messagse
                }
            }
            int subCategoryID = Int32.Parse(raw_subCategory);
            Product.SubCategoryId = subCategoryID;
            int supplierID = Int32.Parse(raw_supplierID);
            Product.SupplierId = supplierID;
            //default
            Product.Active = true;

            // process for product have date
            string raw_dateOfManufacture = req.Form["dateOfManufactureUpdate"];
            string raw_expiry = req.Form["expiryUpdate"];
            if (string.IsNullOrEmpty(raw_expiry) || string.IsNullOrEmpty(raw_dateOfManufacture))
            {
                Product.HaveDate = false;
            }
            else
            {
                Product.HaveDate = true;
                DateTime dateOfManufacture = DateTime.Parse(raw_dateOfManufacture);
                DateTime expiry = DateTime.Parse(raw_expiry);
                DateTime dateNow = DateTime.Now;
                if (dateOfManufacture.CompareTo(dateNow) > 0 || expiry.CompareTo(dateOfManufacture) < 0 || expiry.CompareTo(dateNow) < 0)
                {
                    //message
                }
                else
                {
                    Product.DateOfManufacture = dateOfManufacture;
                    Product.Expiry = expiry;
                }
            }


            //update data
            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
