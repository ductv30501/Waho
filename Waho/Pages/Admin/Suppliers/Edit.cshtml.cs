using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
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
        public Supplier Supplier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            return Page();
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            var req = HttpContext.Request;
            //get data form form submit 
            string raw_supplierID = req.Form["supplierIDUpdate"];
            string raw_conpanyName = req.Form["companyNameUpdate"];
            string raw_addres = req.Form["addressUpdate"];
            string raw_city = req.Form["cityUpdate"];
            string raw_region = req.Form["regionUpdate"];
            string raw_phone = req.Form["phoneUpdate"];
            string raw_taxCode = req.Form["taxCodeUpdate"];
            string raw_branch = req.Form["branchUpdate"];
            string raw_description = req.Form["descriptionUpdate"];

            Supplier.SupplierId = Int32.Parse(raw_supplierID);
            //validate
            if (string.IsNullOrEmpty(raw_conpanyName))
            {
                //message
                message = "tên nhà cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.CompanyName = raw_conpanyName;
            if (string.IsNullOrEmpty(raw_addres))
            {
                //message
                message = "địa chỉ cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.Address = raw_addres;
            Supplier.City = raw_city;
            Supplier.Region = raw_region;
            if (string.IsNullOrEmpty(raw_phone))
            {
                //message
                message = "Số điện thoại của cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.Phone = raw_phone;
            if (string.IsNullOrEmpty(raw_taxCode))
            {
                //message
                message = "Mã số thuế của cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.TaxCode = raw_taxCode;
            Supplier.Branch = raw_branch;
            Supplier.Description = raw_description;
            Supplier.Active = true;

            _context.Attach(Supplier).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // success message
            successMessage = "Chỉnh sửa thông tin nhà cung cấp thành công";
            TempData["successMessage"] = successMessage;
            return RedirectToPage("./Index");
        }

    }
}
