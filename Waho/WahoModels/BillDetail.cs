using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class BillDetail
    {
        public int BillId { get; set; }
        
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        
        [JsonProperty("discount")]
        public double Discount { get; set; }

        public virtual Bill Bill { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
