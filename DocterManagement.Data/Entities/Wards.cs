using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Wards
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public Districs Districs { get; set; }
        public Guid DisticId { get; set; }
        public List<Clinics> Clinics { get; set; }
    }
}
