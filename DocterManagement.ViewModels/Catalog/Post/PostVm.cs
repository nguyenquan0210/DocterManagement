using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class PostVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Image { get; set; }
        [Display(Name = "Nội dung")]
        public string Content { get; set; }
        [Display(Name = "Trạng thái")]
        public Status Status { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        [Display(Name = "Bác sĩ")]
        public DoctorVm Doctors { get; set; }
        [Display(Name = "Chủ đề")]
        public MainMenuVm Topic { get; set; }
       
    }
}
