using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
        //[Display (Name = "Tên sản phẩm")]
        //[Required(ErrorMessage = "Bạn cần nhập tên sản phẩm")]
        public string? ProductName { get; set; }
        //[Display(Name = "Giá nhập")]
        //[Required(ErrorMessage = "Bạn cần nhập giá nhập của sản phẩm")]
        //[Range (0, 999999999, ErrorMessage = "Bạn cần nhập giá sản phẩm lớn hơn 0 đồng")]
        public int? ImportPrice { get; set; }
        //[Display(Name = "Đơn giá")]
        //[Required(ErrorMessage = "Bạn cần nhập đơn giá của sản phẩm")]
        //[Range(0, 999999999999, ErrorMessage = "Bạn cần nhập giá sản phẩm lớn hơn 0 đồng")]
        public int? UnitPrice { get; set; }
        //[Display(Name = "Giá tại kho")]
        //[Required(ErrorMessage = "Bạn cần nhập giá tại kho của sản phẩm")]
        //[Range(0, 999999999999, ErrorMessage = "Bạn cần nhập giá sản phẩm lớn hơn 0 đồng")]
        public int? UnitInStock { get; set; }
        public bool? HaveDate { get; set; }
        public DateTime? DateOfManufacture { get; set; }
        public DateTime? Expiry { get; set; }
        //[Display(Name = "Tên thương hiệu")]
        //[Required(ErrorMessage = "Bạn cần nhập tên thương hiệu")]
        public string? Trademark { get; set; }
        //[Display(Name = "Trọng lượng sản phấm")]
        //[Required(ErrorMessage = "Bạn cần nhập {0}")]
        public int? Weight { get; set; }
        //[Display(Name = "Vị trí")]
        //[Required(ErrorMessage = "Bạn cần nhập {0}")]
        public string? Location { get; set; }
        //[Display(Name = "Đơn vị")]
        //[Required(ErrorMessage = "Bạn cần nhập {0}")]
        public string? Unit { get; set; }
        //[Display(Name = "Định mức tồn min")]
        //[Required(ErrorMessage = "Bạn cần nhập {0}")]
        //[Range (0,9999999999, ErrorMessage = "Bạn cần nhập {0} lớn hơn {1}")]
        public int? InventoryLevelMin { get; set; }
        //[Display(Name = "Định mức tồn max")]
        //[Required(ErrorMessage = "Bạn cần nhập {0}")]
        //[Range(0, 9999999999, ErrorMessage = "Bạn cần nhập {0} lớn hơn {1}")]

        public int? InventoryLevelMax { get; set; }
        public string? Description { get; set; }
        public int SubCategoryId { get; set; }
        public int SupplierId { get; set; }
        public bool? Active { get; set; }

        public virtual SubCategory SubCategory { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<InventorySheetDetail> InventorySheetDetails { get; set; }
        public virtual ICollection<OderDetail> OderDetails { get; set; }
        public virtual ICollection<ReturnOrderProduct> ReturnOrderProducts { get; set; }
    }
}
