using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Contact
{
    public class ContactCreateRequest
    {
        [Display(Name = "Họ tên")]
        public string Name { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Nội dung")]
        public string YourMessage { get; set; }
        public int container_post { get; set; }
    }
}
