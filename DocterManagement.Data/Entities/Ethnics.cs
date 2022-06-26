using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Ethnics
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsDeleted { get; set; }

        public List<Patients> Patients { get; set; }
        
    }
}
