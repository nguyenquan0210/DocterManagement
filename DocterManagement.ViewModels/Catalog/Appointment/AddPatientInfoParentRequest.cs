using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AddPatientInfoParentRequest
    {
        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }
        [Display(Name = "Họ Tên")]
        public string Name { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime Dob { get; set; }
        [Display(Name = "Mối quan hệ ")]
        public string RelativeName { get; set; }
        [Display(Name = "Số điện thoại")]
        public string RelativePhone { get; set; }
        [Display(Name = "Số CMT/CMND")]
        public string Identitycard { get; set; }
        [Display(Name = "Phường/xã")]
        public Guid LocationId { get; set; }
        [Display(Name = "Quận/huyện")]
        public Guid DistrictId { get; set; }
        [Display(Name = "Thành phố")]
        public Guid ProvinceId { get; set; }
        [Display(Name = "Dân tộc")]
        public Guid EthnicId { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid? doctorid { get; set; }
        [Display(Name = "Lịch khám")]
        public Guid? scheduleid { get; set; }
        [Display(Name = "E-mail")]
        public string RelativeEmail { get; set; }
    }
}
