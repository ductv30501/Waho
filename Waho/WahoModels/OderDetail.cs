using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class OderDetail
    {
        public int OderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }

        public virtual Oder Oder { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
