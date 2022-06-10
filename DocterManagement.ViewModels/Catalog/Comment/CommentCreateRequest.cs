using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Comment
{
    public class CommentCreateRequest
    {
        public string Description { get; set; }
        public bool CheckLevel { get; set; }
        public Guid? CheckComentId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
}
