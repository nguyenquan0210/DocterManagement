using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Locations
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public bool IsDeleted { get; set; }
        public string Code { get; set; }  
        public Guid ParentId { get; set; }
        public List<Clinics> Clinics { get; set; }
    }
}
