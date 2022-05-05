using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Patient
{
    public class PatientVm
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Img { get; set; }
        public string No { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public bool IsPrimary { get; set; }
        public string RelativeName { get; set; }
        public Guid RelativeRelationshipId { get; set; }
        public string RelativePhone { get; set; }
        public string Identitycard { get; set; }
        public EthnicVm Ethnics { get; set; }
        public LocationVm Location { get; set; }

        public List<AppointmentVm> Appointments { get; set; }
        public List<MedicalRecordVm> MedicalRecords { get; set; }
    }
}
