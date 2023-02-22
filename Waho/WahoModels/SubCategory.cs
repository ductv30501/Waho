using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class SubCategory
    {
        public SubCategory()
        {
            Products = new HashSet<Product>();
        }

        public int SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
