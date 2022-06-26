using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Speciality
{
    public class SpecialityVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        [Display(Name = "Mã chuyên khoa")]
        public string No { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }

        [Display(Name = "Stt")]
        public int SortOrder { get; set; }
        [Display(Name = "Trang thái")]
        public bool IsDeleted { get; set; }
    }
}
