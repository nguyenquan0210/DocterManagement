using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Patients
    {
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public string Img { get; set; }
        public string No { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public bool IsPrimary { get; set; }
        public string RelativeName { get; set; }
        public Guid RelativeRelationshipId { get; set; }
        public string RelativePhone { get; set; }
        public string RelativeEmail { get; set; }
        public string Identitycard { get; set; }
        public Guid? LocationId { get; set; }
        public Guid UserId { get; set; }
        public Guid EthnicId { get; set; }
        public AppUsers AppUsers { get; set; }
        public Ethnics Ethnics { get; set; }
        public Locations Locations { get; set; }

        public List<Appointments> Appointments { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
    }
}
