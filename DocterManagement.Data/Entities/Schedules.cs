using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Schedules
    {
        public Guid Id { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public Status Status { get; set; }
        public DateTime CheckInDate { get; set; }
        public int Qty { get; set; }
        public Doctors Doctors { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<SchedulesSlots> schedulesSlots { get; set; }
    }
}
