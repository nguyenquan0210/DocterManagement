using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.Service;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Doctors
{
    public class DoctorVm
    {
        public Guid UserId { get; set; }
        [Display(Name = "Tên")]
        public string FirstName { get; set; }
        [Display(Name = "Họ")]
        public string LastName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Địa chỉ ")]
        public string FullAddress { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }
        [Display(Name = "Mô tả/Giới thiêu")]
        public string Intro { get; set; }
        [Display(Name = "Mã bác sĩ")]
        public string No { get; set; }
        [Display(Name = "Ghi chú đặt khám")]
        public string Note { get; set; }
        [Display(Name = "Kinh nghiệm")]
        public string Experiences { get; set; }
        [Display(Name = "Giáo dục ")]
        public string Educations { get; set; }
        [Display(Name = "Tình trạng đặt khám")]
        public bool Booking { get; set; }
        [Display(Name = "Giải thưởng")]
        public string Prizes { get; set; }
        [Display(Name = "Đường dẫn")]
        public string Slug { get; set; }
        [Display(Name = "Lượt xem")]
        public int View { get; set; }
        [Display(Name = "Giời tính")]
        public Gender Gender { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime Dob { get; set; }
        [Display(Name = "Từ đầu ngữ")]
        public string Prefix { get; set; }
        [Display(Name = "Địa chỉ bản đồ")]
        public string MapUrl { get; set; }
        [Display(Name = "Quyền phòng khám")]
        public bool IsPrimary { get; set; }
        [Display(Name = "Phường xã")]
        public LocationVm Location { get; set; }
        [Display(Name = "Tên tài khoản")]
        public UserVm User { get; set; }
        [Display(Name = "Chuyên khoa")]
        public List<GetSpecialityVm> GetSpecialities { get; set; }
        [Display(Name = "Phòng khám")]
        public GetClinicVm GetClinic { get; set; }

        [Display(Name = "Đánh giá")]
        public List<RateVm> Rates { get; set; }
        [Display(Name = "Hình ảnh")]
        public List<GalleryVm> Galleries { get; set; }
        [Display(Name = "Dịch vụ")]
        public List<ServiceVm> Services { get; set; }
        [Display(Name = "Giờ làm việc")]
        public string TimeWorking { get; set; }
        [Display(Name = "Địa chỉ đầy đủ")]
        public string FullName { get; set; }
        [Display(Name = "Đặt trước")]
        public int BeforeBookingDay { get; set; }
        [Display(Name = "Đánh giá")]
        public double Rating { get; set; }
    }
}
