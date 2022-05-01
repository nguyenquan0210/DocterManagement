using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class ScheduleCreateRequest
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public int Qty { get; set; }
        public DateTime FromDay { get; set; }
        public DateTime ToDay { get; set; }
        public string? Username { get; set; }
        public string WeekDay { get; set; }
    }
}
