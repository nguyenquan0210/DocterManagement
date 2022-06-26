using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Patient
{
    public class RegisterPatientSession
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NoOTP { get; set; }
        public DateTime dateTime { get; set; }
    }
}
