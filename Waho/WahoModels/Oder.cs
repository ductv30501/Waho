using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Oder
    {
        public Oder()
        {
            OderDetails = new HashSet<OderDetail>();
        }

        public int OderId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public int ShipperId { get; set; }
        public string? OderState { get; set; }
        public string? Region { get; set; }
        public string? Cod { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? EstimatedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Shipper Shipper { get; set; }
        public virtual Employee UserNameNavigation { get; set; }
        public virtual ICollection<OderDetail> OderDetails { get; set; }
    }
}
