using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Doctors
{
    public class DoctorVm
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public string No { get; set; }
        public Guid SpecialityId { get; set; }
        public Guid ClinicId { get; set; }
    }
}
