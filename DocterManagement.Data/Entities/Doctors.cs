using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Doctors
    {
        public Guid UserId { get; set; }
        
        public string Address { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public string No { get; set; }
        public Guid SpecialityId { get; set; }
        public Guid ClinicId { get; set; }

        public AppUsers AppUsers { get; set; }
        public Specialities Specialities { get; set; }
        public Clinics Clinics { get; set; }

        public List<Posts> Posts { get; set; }
        public List<Schedules> Schedules { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
    }
}
