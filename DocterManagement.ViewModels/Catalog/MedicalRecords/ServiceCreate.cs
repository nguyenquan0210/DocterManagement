using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class ServiceCreate
    {
        public Guid ServiceId { get; set; }
        public int Qty { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Unit { get; set; }
        public Guid AppointmentId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? TotalAmountString { get; set; }
    }
}
