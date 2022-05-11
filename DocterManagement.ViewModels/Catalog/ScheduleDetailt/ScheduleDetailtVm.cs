using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.ScheduleDetailt
{
    public class ScheduleDetailtVm
    {
        public Guid Id { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public Guid ScheduleId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBooked { get; set; }
    }
}
