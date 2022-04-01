using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.ScheduleDetailt
{
    public class ScheduleDetailtCreateRequest
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public Guid ScheduleId { get; set; }
    }
}
