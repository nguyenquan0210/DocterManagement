using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        public string FullAddress { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }
        [Display(Name = "Mô tả/Giới thiêu")]
        public string Intro { get; set; }
        [Display(Name = "Mã bác sĩ")]
        public string No { get; set; }
        public string Note { get; set; }
        public string Experiences { get; set; }
        public string Educations { get; set; }
        public bool Booking { get; set; }
        public string Prizes { get; set; }
        public string Slug { get; set; }
        public int View { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Prefix { get; set; }
        public string Services { get; set; }
        public string MapUrl { get; set; }
        public bool IsPrimary { get; set; }
        public LocationVm Location { get; set; }
        public UserVm User { get; set; }
        public List<GetSpecialityVm> GetSpecialities { get; set; }
        public GetClinicVm GetClinic { get; set; }

        public List<RateVm> Rates { get; set; }
        public List<GalleryVm> Galleries { get; set; }
        public string TimeWorking { get; set; }
        public string FullName { get; set; }
        public int BeforeBookingDay { get; set; }
    }
}
