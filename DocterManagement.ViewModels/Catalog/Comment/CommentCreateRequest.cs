using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Comment
{
    public class CommentCreateRequest
    {
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "STT")]
        public bool CheckLevel { get; set; }
        [Display(Name = "STT")]
        public Guid? CheckComentId { get; set; }
        [Display(Name = "STT")]
        public Guid UserId { get; set; }
        [Display(Name = "STT")]
        public Guid PostId { get; set; }
    }
}
