using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Waho.DataService;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Suppliers
{
    public class CreateModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;
        private readonly Author _author;
        public string message { get; set; }
        public string successMessage { get; set; }
        public CreateModel(Waho.WahoModels.WahoContext context, Author author)
        {
            _context = context;
            _author = author;
        }


        [BindProperty]
        public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //author
            if (!_author.IsAuthor(1))
            {
                return RedirectToPage("/accessDenied", new { message = "Trình quản lý của Admin" });
            }
            return Page();
        }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var req = HttpContext.Request;
            //get data form form submit 
            string raw_conpanyName = req.Form["companyName"];
            string raw_addres = req.Form["address"];
            string raw_city = req.Form["city"];
            string raw_region = req.Form["region"];
            string raw_phone = req.Form["phone"];
            string raw_taxCode = req.Form["taxCode"];
            string raw_branch = req.Form["branch"];
            string raw_description = req.Form["description"];
            //validate
            if (string.IsNullOrEmpty(raw_conpanyName))
            {
                //message
                message = "tên nhà cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.CompanyName= raw_conpanyName;
            if (string.IsNullOrEmpty(raw_addres))
            {
                //message
                message = "địa chỉ cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.Address= raw_addres;
            if (string.IsNullOrEmpty(raw_city))
            {
                //message
                message = "Thành phố cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.City = raw_city;
            if (string.IsNullOrEmpty(raw_region))
            {
                //message
                message = "Khu vực của cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.Region= raw_region;
            if (string.IsNullOrEmpty(raw_phone))
            {
                //message
                message = "Số điện thoại của cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.Phone= raw_phone;
            if (string.IsNullOrEmpty(raw_taxCode))
            {
                //message
                message = "Mã số thuế của cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.TaxCode= raw_taxCode;
            if (string.IsNullOrEmpty(raw_branch))
            {
                //message
                message = "Khi nhánh của cung cấp không được để trống";
                TempData["message"] = message;
                return RedirectToPage("./Index");
            }
            Supplier.Branch= raw_branch;
            Supplier.Description= raw_description;
            Supplier.Active = true;
            // add supplier
            _context.Suppliers.Add(Supplier);
            await _context.SaveChangesAsync();
            // success message
            successMessage = "Thêm mới nhà cung cấp thành công";
            TempData["successMessage"] = successMessage;
            return RedirectToPage("./Index");
        }
    }
}
