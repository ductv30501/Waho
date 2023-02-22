using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Category
    {
        public Category()
        {
            SubCategories = new HashSet<SubCategory>();
            WahoInformations = new HashSet<WahoInformation>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool? HaveDate { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<SubCategory> SubCategories { get; set; }
        public virtual ICollection<WahoInformation> WahoInformations { get; set; }
    }
}
