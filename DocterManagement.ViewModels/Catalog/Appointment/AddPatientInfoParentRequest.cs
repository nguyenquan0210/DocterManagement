using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AddPatientInfoParentRequest
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public string RelativeName { get; set; }
        public string RelativePhone { get; set; }
        public string Identitycard { get; set; }
        public Guid LocationId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid EthnicId { get; set; }
        public Guid? doctorid { get; set; }
        public Guid? scheduleid { get; set; }
        public string RelativeEmail { get; set; }
    }
}
