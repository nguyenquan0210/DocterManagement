using DocterManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; }
        public string Diagnose { get; set; }
        public string Prescription { get; set; }

        public DateTime Date { get; set; }
        public StatusIllness StatusIllness { get; set; }

        public Status Status { get; set; }
        public string Note { get; set; }

        public Patients Patients { get; set; }
        public Guid PatientId { get; set; }
        public Doctors Doctors { get; set; }
        public Guid DoctorId { get; set; }
        public Appointments Appointments { get; set; }
        public Guid AppointmentId { get; set; }
    }
}
