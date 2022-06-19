using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class PostCreateRequest
    {
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Chủ đề")]
        public Guid TopicId { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid DoctorId { get; set; }

        [Display(Name = "Hình ảnh")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Nội dung")]
        public string Content { get; set; }
    }
}
