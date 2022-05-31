using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; }
        public string Diagnose { get; set; }
        public DateTime CreatedAt { get; set; }
        public StatusIllness StatusIllness { get; set; }

        public Status Status { get; set; }
        public string Note { get; set; }
        public decimal TotalAmount { get; set; }

        public Appointments Appointments { get; set; }
        public Guid AppointmentId { get; set; }

        public List<MedicineDetailts> MedicineDetailts { get; set; }
    }
}
