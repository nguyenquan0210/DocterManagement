using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.System.Doctors;
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
        
        [Display(Name = "Giới thiệu")]
        public string Description { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Display(Name = "Trạng thái")]
        public Status Status { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public IFormFile? ThumbnailImage { get; set; }

        [Display(Name = "Chuyên Khoa")]
        public ICollection<Guid> Specialities { get; set; }
        [Display(Name = "Phòng Khám")]
        public Guid ClinicId { get; set; }
        [Display(Name = "Cho phép đặt khám")]
        public bool Booking { get; set; }
        [Display(Name = "MapUrl")]
        public string MapUrl { get; set; }
        [Display(Name = "WordSpress Slug")]
        public string Slug { get; set; }
        [Display(Name = "Tên")]
        public string FirstName { get; set; }
        [Display(Name = "Họ")]
        public string LastName { get; set; }
        [Display(Name = "Quyền quản lý phòng khám")]
        public bool IsPrimary { get; set; }
        [Display(Name = "Tiếp đầu ngữ")]
        public string Prefix { get; set; }
        [Display(Name = "Thành Phố")]
        public Guid ProvinceId { get; set; }
        [Display(Name = "Quận/huyện")]
        public Guid DistrictId { get; set; }
        [Display(Name = "Phường/xã")]
        public Guid SubDistrictId { get; set; }
        [Display(Name = "Dịch vụ khám bệnh")]
        public string Services { get; set; }
        [Display(Name = "Giải thưởng")]
        public string Prizes { get; set; }
        [Display(Name = "Học vấn")]
        public string Educations { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
        [Display(Name = "Giờ làm việc")]
        public string TimeWorking { get; set; }
        [Display(Name = "Hình ảnh")]
        public IFormFileCollection? Galleries { get; set; }
        public List<GalleryVm>? GetGalleries { get; set; }
    }
}
