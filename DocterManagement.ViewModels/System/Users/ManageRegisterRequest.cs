using DoctorManagement.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class ManageRegisterRequest
    {
        [Display(Name = "Họ")]
        public string LastName { get; set; }
        [Display(Name = "Tên")]
        public string FisrtName { get; set; }
        [Display(Name = "Tiếp đầu ngữ")]
        public string Prefix { get; set; }
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }
        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "MapUrl")]
        public string MapUrl { get; set; }
      
        [Display(Name = "WordPress Slug")]
        public string Slug { get; set; }

        [Display(Name = "Chuyên Khoa")]
        public ICollection<Guid> SpecialityId { get; set; }

        [Display(Name = "Phòng Khám")]
        public Guid ClinicId { get; set; }
        [Display(Name = "Thành Phố")]
        public Guid ProvinceId { get; set; }
        [Display(Name = "Quận/huyện")]
        public Guid DistrictId { get; set; }
        [Display(Name = "Phường/xã")]
        public Guid SubDistrictId { get; set; }

    }
}
