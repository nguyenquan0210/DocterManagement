using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class ImagePost
    {
        public Guid Id { get; set; }
        public string Img { get; set; }
        public int SortOrder { get; set; }
        public Posts Posts { get; set; }
        public Guid PostId { get; set; }
    }
}
