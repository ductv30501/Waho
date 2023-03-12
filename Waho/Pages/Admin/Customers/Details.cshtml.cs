﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Waho.WahoModels;

namespace Waho.Pages.Admin.Customers
{
    public class DetailsModel : PageModel
    {
        private readonly Waho.WahoModels.WahoContext _context;

        public DetailsModel(Waho.WahoModels.WahoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            Customer = customer;

            return new JsonResult(Customer);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            string id = HttpContext.Request.Form["id"];
            string name = HttpContext.Request.Form["name"];
            string raw_dob = HttpContext.Request.Form["dob"];
            string phone = HttpContext.Request.Form["phone"];
            string email = HttpContext.Request.Form["email"];
            string raw_type = HttpContext.Request.Form["type"];
            string tax = HttpContext.Request.Form["tax"];
            string address = HttpContext.Request.Form["address"];
            string note = HttpContext.Request.Form["note"];
            string active = HttpContext.Request.Form["active"];

            Customer.CustomerId = int.Parse(id);
            Customer.CustomerName = name;
            Customer.Adress = address;
            Customer.Phone = phone;
            Customer.Email = email;
            Customer.Active = true;
            Customer.Description = note;
            Customer.TaxCode = tax;
            Customer.Dob = DateTime.Parse(raw_dob);
            Customer.TypeOfCustomer = Boolean.Parse(raw_type);
            Customer.Active = bool.Parse(active);

            _context.Attach(Customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(Customer.CustomerId))
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

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
