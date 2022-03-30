using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Patients
    {
        public Guid UserId { get; set; }
     
        public string Address { get; set; }
        public string Img { get; set; }
        public AppUsers AppUsers { get; set; }

        public List<Appointments> Appointments { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
    }
}
