using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentVm
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Stt { get; set; }
        public string No { get; set; }
        public string Note { get; set; }
        public bool IsDoctor { get; set; }
        public StatusAppointment Status { get; set; }
        public ScheduleVm Schedule { get; set; }
        public SlotScheduleVm SlotSchedule { get; set; }
        public PatientVm Patient { get; set; }
        public DoctorVm Doctor { get; set; }
        public MedicalRecordVm MedicalRecord { get; set; }
        public RateVm Rate { get; set; }
        public string CancelReason { get; set; }
    }
}
