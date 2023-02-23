using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class InventorySheetDetail
    {
        public int InventorySheetId { get; set; }
        public int ProductId { get; set; }
        public int? CurNwareHouse { get; set; }

        public virtual InventorySheet InventorySheet { get; set; }
        public virtual Product Product { get; set; }
    }
}
