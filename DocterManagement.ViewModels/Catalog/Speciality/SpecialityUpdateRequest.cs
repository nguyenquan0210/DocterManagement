using DoctorManagement.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Speciality
{
    public class SpecialityUpdateRequest
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Stt")]
        public int SortOrder { get; set; }
        [Display(Name = "Trạng trạng")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Hình ảnh")]
        public IFormFile? Img { get; set; }
    }
}
