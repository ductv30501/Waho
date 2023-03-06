using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class InventorySheet
    {
        public InventorySheet()
        {
            InventorySheetDetails = new HashSet<InventorySheetDetail>();
        }

        public int InventorySheetId { get; set; }
        public string? Description { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime? Date { get; set; }

        public virtual Employee UserNameNavigation { get; set; } = null!;
        public virtual ICollection<InventorySheetDetail> InventorySheetDetails { get; set; }
    }
}
