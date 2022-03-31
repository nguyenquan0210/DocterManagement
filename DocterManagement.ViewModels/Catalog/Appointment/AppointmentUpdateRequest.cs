using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentUpdateRequest
    {
        public Guid Id { get; set; }

        public StatusAppointment Status { get; set; }
    }
}
