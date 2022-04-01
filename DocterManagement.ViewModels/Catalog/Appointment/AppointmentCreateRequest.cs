using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentCreateRequest
    {
        public Guid SchedulesDetailId { get; set; }
        public Guid PatientId { get; set; }
    }
}
