using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Location;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicUpdateRequest
    {
        [Required]
        [Display(Name = "Tên phòng khám")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public IFormFile? ImgLogo { get; set; }
        [Required]
        [Display(Name = "Mô ta")]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Phường xã")]
        public Guid LocationId { get; set; }

        [Required]
        [Display(Name = "Quận huyện")]
        public Guid DistrictId { get; set; }

        [Display(Name = "Hình ảnh")]
        public IFormFileCollection? ImgClinics { get; set; }
        public Guid Id { get; set; }
        [Display(Name = "Trạng thái")]
        public Status Status { get; set; }

        [Display(Name = "Hình ảnh")]
        public List<ImageClinicVm>? Images { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        [Display(Name = "Địa chỉ bản đồ")]
        public string MapUrl { get; set; }
        [Display(Name = "Dịch vụ")]
        public string? Service { get; set; }
        [Display(Name = "Liên hệ")]
        public string? Contact { get; set; }
    }
}
