using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Waho.WahoModels
{
    public partial class Employee
    {
        public Employee()
        {
            Bills = new HashSet<Bill>();
            InventorySheets = new HashSet<InventorySheet>();
            Oders = new HashSet<Oder>();
            ReturnOrders = new HashSet<ReturnOrder>();
        }

        [Display(Name ="Tên đăng nhập")]
        public string UserName { get; set; }
        public string? EmployeeName { get; set; }
        public string? Title { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
        public string? Region { get; set; }
        public string? Phone { get; set; }
        public string? Note { get; set; }

        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }
        public int? WahoId { get; set; }
        public int? Role { get; set; }

        public virtual WahoInformation Waho { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<InventorySheet> InventorySheets { get; set; }
        public virtual ICollection<Oder> Oders { get; set; }
        public virtual ICollection<ReturnOrder> ReturnOrders { get; set; }
    }
}
