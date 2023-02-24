using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Customer
    {
        public Customer()
        {
            Bills = new HashSet<Bill>();
            Oders = new HashSet<Oder>();
            ReturnOrders = new HashSet<ReturnOrder>();
        }

        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }
        public string Adress { get; set; }
        public bool? TypeOfCustomer { get; set; }
        public string? TaxCode { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Oder> Oders { get; set; }
        public virtual ICollection<ReturnOrder> ReturnOrders { get; set; }
    }
}
