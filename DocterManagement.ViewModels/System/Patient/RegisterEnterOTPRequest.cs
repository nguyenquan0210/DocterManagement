using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Patient
{
    public class RegisterEnterOTPRequest
    {
        public string PhoneNumber { get; set; }
        public string Otp_1 { get; set; }
        public string Otp_2 { get; set; }
        public string Otp_3 { get; set; }
        public string Otp_4 { get; set; }
        public string Otp_5 { get; set; }
        public string Otp_0 { get; set; }
    }
}
