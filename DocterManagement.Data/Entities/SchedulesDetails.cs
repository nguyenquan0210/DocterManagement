﻿using DocterManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Entities
{
    public class SchedulesDetails
    {
        public Guid Id { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public Status Status { get; set; }
        public Schedules Schedules { get; set; }
        public Guid ScheduleId { get; set; }
        public Appointments Appointments { get; set; }
    }
}
