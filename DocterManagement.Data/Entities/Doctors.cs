using DoctorManagement.Data.Enums;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Img { get; set; }
        public string Intro { get; set; }
        public string No { get; set; }
        public string Note { get; set; }
        public string TimeWorking { get; set; }
        public string Educations { get; set; }
        public bool Booking { get; set; }
        public string Prizes { get; set; }
        public string Slug { get; set; }
        public int View { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Prefix { get; set; }
        public string Services { get; set; }
        public string MapUrl { get; set; }
        public bool IsPrimary { get; set; }
        public Guid LocationId { get; set; }
        public Guid ClinicId { get; set; }

        public AppUsers AppUsers { get; set; }
        
        public Clinics Clinics { get; set; }
        public Locations Locations { get; set; }

        public List<Posts> Posts { get; set; }
        public List<Schedules> Schedules { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
        public List<Rates> Rates { get; set; }
        public List<Galleries> Galleries { get; set; }

        public List<ServicesSpecialities> ServicesSpecialities { get; set; }
        public List<Appointments> Appointments { get; set; }
    }
}
