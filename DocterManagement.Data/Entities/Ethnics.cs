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

        public List<Patients> Patients { get; set; }
        
    }
}
