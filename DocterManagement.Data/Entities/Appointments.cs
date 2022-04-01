using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Appointments
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        
        public StatusAppointment Status { get; set; }
        public Guid SchedulesDetailId { get; set; }
        public Guid PatientId { get; set; }

        public Patients Patients { get; set; }
        public SchedulesDetails SchedulesDetails { get; set; }
        public MedicalRecord MedicalRecords { get; set; }

        public Rates Rates { get; set; }
    }
}
