using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class MedicalRecordCreateRequest
    {
        public string Diagnose { get; set; }
        public List<MedicineCreate>? Medicine { get; set; }
        public List<ServiceCreate> Service{ get; set; }

        public StatusIllness StatusIllness { get; set; }

        public string? Note { get; set; }
        public Guid AppointmentId { get; set; }
    }
}
