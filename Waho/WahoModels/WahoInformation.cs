using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Waho.WahoModels
{
    public partial class WahoInformation
    {
        public WahoInformation()
        {
            Employees = new HashSet<Employee>();
        }

        public int WahoId { get; set; }

        [Display(Name = "Tên cửa hàng")]
        public string? WahoName { get; set; }

        [Display(Name = "Địa chỉ cửa hàng")]
        public string? Address { get; set; }
        public int CategoryId { get; set; }

        [Display(Name = "Điện thoại")]
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
