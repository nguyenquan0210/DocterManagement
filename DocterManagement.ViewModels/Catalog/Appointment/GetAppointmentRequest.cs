using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class GetAppointmentRequest
    {
        public string? Keyword { get; set; }
        public string? UserName { get; set; }
        public string? UserNameDoctor { get; set; }
        public bool Rating { get; set; }
        public StatusAppointment? status { get; set; }
    }
}
