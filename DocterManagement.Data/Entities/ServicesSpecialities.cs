using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class ServicesSpecialities
    {
        public Guid Id { get; set; }
        public bool IsDelete { get; set; }
        public Guid DoctorId { get; set; }
        public Guid SpecialityId { get; set; }

        public Specialities Specialities { get; set; }
        public Doctors Doctors { get; set; }


    }
}
