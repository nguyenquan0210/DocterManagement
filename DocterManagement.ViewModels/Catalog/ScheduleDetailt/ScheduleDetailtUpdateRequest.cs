using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.ScheduleDetailt
{
    public class ScheduleDetailtUpdateRequest
    {
        public Guid Id { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
