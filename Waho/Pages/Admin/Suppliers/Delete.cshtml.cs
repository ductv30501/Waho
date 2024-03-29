﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
{
    public class DeleteModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public string message { get; set; }
        public string successMessage { get; set; }
        public DeleteModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }

        [BindProperty]
        public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnGetAsync(int? supplierID)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.SupplierId == supplierID);
            if (supplier != null)
            {
                Supplier = supplier;
                Supplier.Active = false;
                _context.Attach(Supplier).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // message
                successMessage = "Xóa thành công nhà cung cấp ra khỏi danh sách";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            message = "không tìm thấy nhà cung cấp";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }


    }
}
