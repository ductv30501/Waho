using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Product
    {
        public Product()
        {
            BillDetails = new HashSet<BillDetail>();
            InventorySheetDetails = new HashSet<InventorySheetDetail>();
            OderDetails = new HashSet<OderDetail>();
            ReturnOrderProducts = new HashSet<ReturnOrderProduct>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ImportPrice { get; set; }
        public int? UnitPrice { get; set; }
        public int? UnitInStock { get; set; }
        public bool? HaveDate { get; set; }
        public DateTime? DateOfManufacture { get; set; }
        public DateTime? Expiry { get; set; }
        public string? Trademark { get; set; }
        public int? Weight { get; set; }
        public string? Location { get; set; }
        public string? Unit { get; set; }
        public int? InventoryLevelMin { get; set; }
        public int? InventoryLevelMax { get; set; }
        public string? Description { get; set; }
        public int SubCategoryId { get; set; }
        public int SupplierId { get; set; }

        public virtual SubCategory SubCategory { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<InventorySheetDetail> InventorySheetDetails { get; set; }
        public virtual ICollection<OderDetail> OderDetails { get; set; }
        public virtual ICollection<ReturnOrderProduct> ReturnOrderProducts { get; set; }
    }
}
