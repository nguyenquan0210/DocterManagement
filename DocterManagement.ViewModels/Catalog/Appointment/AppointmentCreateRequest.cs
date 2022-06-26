using DoctorManagement.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentCreateRequest
    {
        [Display(Name = "Khung giờ khám")]
        public Guid SchedulesSlotId { get; set; }
        [Display(Name = "Hồ sơ")]
        public Guid PatientId { get; set; }
        [Display(Name = "Phòng khám")]
        public Guid? ClinicId { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid DoctorId { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        public bool IsDoctor { get; set; }
        public IFormFileCollection? formFiles { get; set; }
    }
}
