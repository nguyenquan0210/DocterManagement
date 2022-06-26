using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class MedicalRecordCreateRequest
    {
        [Display(Name = "Chuẩn đoán bệnh")]
        public string Diagnose { get; set; }
        [Display(Name = "Toa thuốc")]
        public List<MedicineCreate>? Medicine { get; set; }
        [Display(Name = "Dịch vụ khám")]
        public List<ServiceCreate> Service{ get; set; }

        [Display(Name = "Tình trạng bệnh")]
        public StatusIllness StatusIllness { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        [Display(Name = "Lịch hẹn")]
        public Guid AppointmentId { get; set; }
    }
}
