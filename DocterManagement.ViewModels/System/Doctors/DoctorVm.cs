using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Speciality;
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
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }
        [Display(Name = "Mô tả/Giới thiêu")]
        public string Description { get; set; }
        [Display(Name = "Mã bác sĩ")]
        public string No { get; set; }
        public GetSpecialityVm GetSpeciality { get; set; }
        public GetClinicVm GetClinic { get; set; }
    }
}
