using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Service
{
    public class ServiceDetailtVm
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid ServicesId { get; set; }
        [Display(Name = "Số lượng")]
        public int Qty { get; set; }

        public MedicalRecordVm MedicalRecord { get; set; }
        public ServiceVm Services { get; set; }
    }
}
