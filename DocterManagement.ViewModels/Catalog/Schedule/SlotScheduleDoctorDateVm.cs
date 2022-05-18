using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class SlotScheduleDoctorDateVm
    {
        public int type { get; set; }
        public string timeSpan { get; set; }
        public Guid slotId { get; set; }
        
    }
}
