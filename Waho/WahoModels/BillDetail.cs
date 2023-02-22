using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class BillDetail
    {
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
        public double? Discount { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual Product Product { get; set; }
    }
}
