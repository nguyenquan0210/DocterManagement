using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Galleries
    {
        public Guid Id { get; set; }
        public string Img { get; set; }
        public int SortOrder { get; set; }
        
        public Guid DoctorId { get; set; }
        public Doctors Doctors { get; set; }
    }
}
