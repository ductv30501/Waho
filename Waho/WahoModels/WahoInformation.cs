using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class WahoInformation
    {
        public WahoInformation()
        {
            Employees = new HashSet<Employee>();
        }

        public int WahoId { get; set; }
        public string? WahoName { get; set; }
        public string? Address { get; set; }
        public int CategoryId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
