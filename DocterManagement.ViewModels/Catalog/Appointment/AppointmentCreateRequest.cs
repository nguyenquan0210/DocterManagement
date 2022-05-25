using DoctorManagement.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentCreateRequest
    {
        public Guid SchedulesSlotId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? ClinicId { get; set; }
        public Guid DoctorId { get; set; }
        public string? Note { get; set; }
        public bool IsDoctor { get; set; }
        public IFormFileCollection? formFiles { get; set; }
    }
}
