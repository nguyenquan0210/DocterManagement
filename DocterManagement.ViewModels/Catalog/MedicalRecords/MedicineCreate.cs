using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class MedicineCreate
    {
        public Guid MedicineId { get; set; }
        public int Qty { get; set; }
        public string Use { get; set; }
        public string? Unit { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? TotalAmountString { get; set; }
        public int? Morning { get; set; }
        public int? Noon { get; set; }
        public int? Afternoon { get; set; }
        public int? Night { get; set; }
        public Guid AppointmentId { get; set; }

    }
}
