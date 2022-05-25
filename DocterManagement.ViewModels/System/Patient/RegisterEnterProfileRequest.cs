using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Patient
{
    public class RegisterEnterProfileRequest
    {
        public string RelativePhone { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public DateTime Dob { get; set; }
        public Gender Gender { get; set; }
        public string IdentityCard { get; set; }
        public string Email { get; set; }

        public Guid EthnicGroupId { get; set; }
        public string Address { get; set; }
        public Guid SubDistrictId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid ProvinceId { get; set; }
    }
}
