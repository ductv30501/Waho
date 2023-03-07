﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Cashier.ReturnOrders
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
      public ReturnOrder ReturnOrder { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var _returnOrder = await _context.ReturnOrders.FirstOrDefaultAsync(m => m.ReturnOrderId == id);
            if (_returnOrder != null)
            {
                ReturnOrder = _returnOrder;
                ReturnOrder.Active = false;
                _context.Attach(ReturnOrder).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                // message
                successMessage = "Xóa thành công phiếu hoàn đơn ra khỏi danh sách";
                TempData["successMessage"] = successMessage;
                return RedirectToPage("./Index");
            }
            message = "không tìm thấy phiếu hoàn đơn";
            TempData["message"] = message;
            return RedirectToPage("./Index");
        }

    }
}
