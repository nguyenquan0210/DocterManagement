using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Contact
{
    public class ContactVm
    {
        public Guid Id { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Nội dung")]
        public string YourMessage { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CratedAt { get; set; }
        [Display(Name = "STT")]
        public string Name { get; set; }
    }
}
