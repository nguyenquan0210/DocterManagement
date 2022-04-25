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
    public class UserUpdateRequest
    {
        public Guid Id { get; set; }

        [Display(Name = "Họ Tên")]
        public string Name { get; set; }
        [Display(Name = "Giới thiệu")]
        public string Description { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Display(Name = "Hòm thư")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Trạng thái")]
        public Status Status { get; set; }

        [Display(Name = "Hình ảnh")]
        public IFormFile? ThumbnailImage { get; set; }
        public string? img { get; set; }

        [Display(Name = "Chuyên Khoa")]
        public Guid SpecialityId { get; set; }
        [Display(Name = "Phòng Khám")]
        public Guid ClinicId { get; set; }
    }
}
