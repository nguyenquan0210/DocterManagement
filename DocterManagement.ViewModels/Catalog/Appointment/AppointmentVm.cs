using DoctorManagement.Data.Enums;
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
        public DateTime Date { get; set; }
        public string No { get; set; }
        public StatusAppointment Status { get; set; }
        public Guid SchedulesDetailId { get; set; }
        public Guid PatientId { get; set; }
    }
}
