using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class DoctorScheduleClientsVm
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public int AvailableQty { get; set; }
        public Guid DoctorId { get; set; }
        public List<ScheduleSlotsVm> ScheduleSlots { get; set; }
        public int CountTimeSpan { get; set; }
    }
}
