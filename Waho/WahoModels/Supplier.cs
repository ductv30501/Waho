using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
        }

        public int SupplierId { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Phone { get; set; }
        public string? TaxCode { get; set; }
        public string? Branch { get; set; }
        public string? Description { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
