using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class ServiceDetailts
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid ServicesId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public Services Services { get; set; }
    }
}
