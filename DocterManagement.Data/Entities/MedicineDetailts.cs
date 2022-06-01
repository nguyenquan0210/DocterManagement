using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class MedicineDetailts
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid MedicineId { get; set; }
        public int Qty { get; set; }
        public string Use { get; set; }
        public int Morning { get; set; }
        public int Noon { get; set; }
        public int Afternoon { get; set; }
        public int Night { get; set; }

        public MedicalRecord MedicalRecord { get; set; }
        public Medicines Medicine { get; set; }
    }
}
