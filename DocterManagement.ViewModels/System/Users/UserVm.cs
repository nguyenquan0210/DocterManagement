using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class UserVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Họ Tên")]
        public string Name { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime Dob { get; set; }

        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; }

        [Display(Name = "Trạng thái")]
        public Status Status { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }

        public GetRoleVm GetRole { get; set; }

        public DoctorVm? DoctorVm { get; set; }
        public PatientVm? PatientVm { get; set; }
        [Display(Name = "Ngày đăng ký")]
        public DateTime Date { get; set; }
        
    }
}
