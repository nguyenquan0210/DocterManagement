using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Patient
{
    public class PatientVm
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string Img { get; set; }
        public string No { get; set; }
    }
}
