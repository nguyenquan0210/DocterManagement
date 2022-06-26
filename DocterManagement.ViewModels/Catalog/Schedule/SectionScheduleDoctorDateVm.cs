using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class SectionScheduleDoctorDateVm
    {
        public int type { get; set; }
        public string text { get; set; }
        public string img { get; set; }
        public List<SlotScheduleDoctorDateVm> slot { get; set; }
    }
}
