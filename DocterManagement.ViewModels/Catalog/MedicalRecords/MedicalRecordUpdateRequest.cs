using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class MedicalRecordUpdateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Chuẩn đoán bệnh")]
        public string Diagnose { get; set; }
        [Display(Name = "Toa thuốc")]
        public string Prescription { get; set; }

        [Display(Name = "Tình trạng bệnh")]
        public StatusIllness StatusIllness { get; set; }

        [Display(Name = "Tình trạng")]
        public Status Status { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

    }
}
