using DocterManagement.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Entities
{
    public class AppUsers : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public Status Status { get; set; }
        public Guid Roles { get; set; }

        public AppRoles AppRoles { get; set; }
        public Guid RoleId { get; set; }
        public Doctors Doctors { get; set; }
        public Patients Patients { get; set; }
        public List<CommentsPost> CommentsPosts { get; set; }


    }
}
