using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentUpdateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Trạng thái")]
        public StatusAppointment Status { get; set; }
    }
}
