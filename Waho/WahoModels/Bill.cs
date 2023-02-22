using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Bill
    {
        public int BillId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public DateTime? Date { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee UserNameNavigation { get; set; }
    }
}
