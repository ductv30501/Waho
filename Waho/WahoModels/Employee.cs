using System;
using System.Collections.Generic;

namespace Waho.WahoModels
{
    public partial class Employee
    {
        public Employee()
        {
            Bills = new HashSet<Bill>();
            Oders = new HashSet<Oder>();
        }

        public string UserName { get; set; }
        public string EmployeeName { get; set; }
        public string Title { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? HireDate { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string Password { get; set; }
        public int? WahoId { get; set; }
        public int? Role { get; set; }

        public virtual WahoInformation Waho { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Oder> Oders { get; set; }
    }
}
