﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        public string message { get; set; }
        public string successMessage { get; set; }
        public DeleteModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnGetAsync(string? supplierID)
        {
            int supplierId = Int32.Parse(supplierID);
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.SupplierId == supplierId);
            if (supplier != null)
            {
                Supplier = supplier;
                Supplier.Active = false;
                _context.Attach(Supplier).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // message
                successMessage = "Xóa thành công nhà cung cấp ra khỏi danh sách";
                TempData["successMessage"] = successMessage;
            }
            message = "không tìm thấy sản phẩm";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }


    }
}
