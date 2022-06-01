using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Services
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public string ServiceName { get; set; }
        public Guid DoctorId { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }

        public Doctors Doctor { get; set; }

        public List<ServiceDetailts> ServiceDetailts { get; set; }
    }
}
