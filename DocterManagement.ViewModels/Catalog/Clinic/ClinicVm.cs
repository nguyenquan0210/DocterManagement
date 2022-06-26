using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Mã phòng khám")]
        public string No { get; set; }
        [Display(Name = "Tên phòng khám")]
        public string Name { get; set; }
        [Display(Name = "Logo")]
        public string ImgLogo { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Phường xã")]
        public LocationVm LocationVm { get; set; }
        [Display(Name = "Trạng thái")]
        public Status Status { get; set; }
        [Display(Name = "Hình ảnh")]
        public List<ImageClinicVm> Images { get; set; }
        [Display(Name = "Danh sách bác sĩ")]
        public List<DoctorVm> DoctorVms { get; set; }
        [Display(Name = "Địac chỉ đầy đủ")]
        public string FullAddress { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Địa chỉ bản đồ")]
        public string MapUrl { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }
        public string RatingText{ get; set; }
        [Display(Name = "Dịch vụ")]
        public string Service { get; set; }
        [Display(Name = "Liên hệ")]
        public string Contact { get; set; }
    }
}
