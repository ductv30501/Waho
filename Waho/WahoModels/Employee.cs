using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

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
        [Required(ErrorMessage = "Phải có tên tài khoản")]
        [MinLength(6, ErrorMessage = "phải có ít nhất 6 ký tự")]
        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Phải có tên nhân viên")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Chiều dài tên phải từ 3 đến 50 ký tự")]
        [Display(Name = "Tên nhân viên")]
        public string EmployeeName { get; set; }
        public string Title { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
        public string? Region { get; set; }
        public string? Phone { get; set; }
        public string? Note { get; set; }
        [Required(ErrorMessage = "Phải có mật khẩu")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Chiều dài mật khẩu phải từ 6 đến 50 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        public int? WahoId { get; set; }
        public int? Role { get; set; }
        [Required(ErrorMessage = "Phải có email")]
        [EmailAddress(ErrorMessage = "phải nhập đúng định dạng email vd: youremail@gmail.com")]
        [Display(Name = "EMAIL")]
        public string Email { get; set; }

        public virtual WahoInformation Waho { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<InventorySheet> InventorySheets { get; set; }
        public virtual ICollection<Oder> Oders { get; set; }
        public virtual ICollection<ReturnOrder> ReturnOrders { get; set; }
    }
}
