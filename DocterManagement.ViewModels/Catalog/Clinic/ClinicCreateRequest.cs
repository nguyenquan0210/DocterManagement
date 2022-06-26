using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicCreateRequest
    {
        [Required]
        [Display(Name = "Tên phòng khám")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Logo")]
        public IFormFile ImgLogo { get; set; }
        [Required]
        [Display(Name = "Mô tả")]
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

        [Required]
        [Display(Name = "Hình ảnh")]
        public IFormFileCollection ImgClinics { get; set; }
        [Display(Name = "Địa chỉ bản đồ")]
        public string MapUrl { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        [Display(Name = "Dịch vụ")]
        public string? Service { get; set; }
        [Display(Name = "Liện hệ")]
        public string? Contact { get; set; }
    }
}
