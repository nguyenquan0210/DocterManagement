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
        public DateTime CheckInDate { get; set; }
        public int Qty { get; set; }
        public Guid DoctorId { get; set; }
    }
}
