using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Attachedfiles
    {
        public Guid Id { get; set; }
        public string Img { get; set; }
        public Guid AppointmentId { get; set; }
        public Appointments Appointments { get; set; }
    }
}
