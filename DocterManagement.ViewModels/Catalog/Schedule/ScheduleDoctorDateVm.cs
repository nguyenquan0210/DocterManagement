using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class ScheduleDoctorDateVm
    {
        public string DateTime { get; set; }
        public int day { get; set; }
        public bool IsActive { get; set; }
        public bool DateNow { get; set; }
    }
}
