using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Posts
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public Doctors Doctors { get; set; }
        public Guid DoctorId { get; set; }

        public List<ImagePost> ImagePosts { get; set; }
        public List<CommentsPost> CommentsPosts { get; set; }
    }
}
