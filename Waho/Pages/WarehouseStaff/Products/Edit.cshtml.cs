﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.WarehouseStaff.Products
{
    public class EditModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public string message { get; set; }
        public string successMessage { get; set; }
        public EditModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(3))
            {
                return RedirectToPage("/accessDenied", new { message = "Quản lý sản phẩm" });
            }
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
            string raw_quantity = req.Form["quantityUpdate"];
            string raw_weight = req.Form["weightUpdate"];
            string raw_location = req.Form["locationUpdate"];
            string raw_unit = req.Form["unitUpdate"];
            string raw_inventoryLevelMin = req.Form["inventoryLevelMinUpdate"];
            string raw_inventoryLevelMax = req.Form["inventoryLevelMaxUpdate"];
            string raw_description = req.Form["descriptionUpdate"];
            string raw_subCategory = req.Form["subCategoryUpdate"];
            string raw_supplierID = req.Form["supplierIDUpdate"];

            Product.ProductId = Int32.Parse(raw_productID);
            Product = _context.Products.Find(Product.ProductId);
            Product.ProductName = raw_productName;
            if (!string.IsNullOrWhiteSpace(raw_importPrice))
            {
                int import_Price = int.Parse(raw_importPrice);
                if (import_Price > 0)
                {
                    Product.ImportPrice = import_Price;
                }
                else
                {
                    // messagse
                    message = "Bạn cần nhập giá lớn hơn 0 đồng";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
            }
            if (!string.IsNullOrWhiteSpace(raw_unitInStock))
            {
                int unitInStock = int.Parse(raw_unitInStock);
                if (unitInStock > 0)
                {
                    Product.UnitInStock = unitInStock;
                }
                else
                {
                    // messagse
                    message = "Bạn cần nhập đơn vị giá tại cửa hàng lớn hơn 0 đồng";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
            }
            if (!string.IsNullOrWhiteSpace(raw_unitPrice))
            {
                if (int.Parse(raw_unitPrice) > 0)
                {
                    Product.UnitPrice = int.Parse(raw_unitPrice);
                }
                else
                {
                    // messagse
                    message = "bạn cần nhập đơn vị giá lớn hơn 0 đồng";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
            }
            Product.Trademark = raw_trademark;
            if (!string.IsNullOrWhiteSpace(raw_quantity))
            {
                int quantity = int.Parse(raw_quantity);
                if (quantity > 0)
                {
                    Product.Quantity = quantity;
                }
                else
                {
                    // messagse
                    message = "bạn cần nhập số lượng lớn hơn 0";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
            }
            if (!string.IsNullOrWhiteSpace(raw_weight))
            {
                if (int.Parse(raw_weight) > 0)
                {
                    Product.Weight = int.Parse(raw_weight);
                }
                else
                {
                    // messagse
                    message = "bạn cần nhập trọng lượng lớn hơn 0 đồng";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
            }
            Product.Location = raw_location;
            Product.Description = raw_description;
            Product.Unit = raw_unit;
            if (!string.IsNullOrWhiteSpace(raw_inventoryLevelMin))
            {
                int inventoryLevelMin = int.Parse(raw_inventoryLevelMin);
                if (inventoryLevelMin > 0)
                {
                    Product.InventoryLevelMin = inventoryLevelMin;
                }
                else
                {
                    // messagse
                    message = "bạn cần nhập số lượng tồn nhỏ nhất là 1";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
            }
            if (!string.IsNullOrWhiteSpace(raw_inventoryLevelMax))
            {
                int inventoryLevelMax = int.Parse(raw_inventoryLevelMax);
                if (inventoryLevelMax > 0)
                {
                    Product.InventoryLevelMax = inventoryLevelMax;
                }
                else
                {
                    // messagse
                    message = "bạn cần nhập số lượng tồn nhỏ lớn nhất phải lớn hơn 0 ";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
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
                    message = "bạn không được nhập HSD nhỏ hơn NSX hoặc NXS,HSD nhỏ hơn thời gian hiện tại";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
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
                    message = "không tồn tại sản phẩm này";
                    TempData["message"] = message;
                    return RedirectToPage("./Index");
                }
                else
                {
                    throw;
                }
            }
            successMessage = "Đã sửa thành công thông tin sản phẩm";
            TempData["successMessage"] = successMessage;
            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
