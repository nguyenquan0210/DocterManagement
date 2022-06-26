using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class MedicalRecordVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Chuẩn đoán bệnh")]
        public string Diagnose { get; set; }
        [Display(Name = "Toa thuốc")]
        public List<MedicineCreate>? Medicine { get; set; }
        [Display(Name = "Dich vụ khám")]
        public List<ServiceCreate> Service { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreateAt { get; set; }
        [Display(Name = "Tình trạng bệnh")]
        public StatusIllness StatusIllness { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Tình trạng")]
        public Status Status { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Bệnh nhân")]
        public Guid PatientId { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid DoctorId { get; set; }
        [Display(Name = "Lịch hẹn")]
        public Guid AppointmentId { get; set; }
    }
}
