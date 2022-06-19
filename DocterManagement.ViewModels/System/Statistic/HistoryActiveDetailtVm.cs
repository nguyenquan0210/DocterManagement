using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Statistic
{
    public class HistoryActiveDetailtVm
    {
        public Guid Id { get; set; }
        public string MethodName { get; set; }
        public Guid HistoryActiveId { get; set; }
        public string ServiceName { get; set; }
        public string Parameters { get; set; }
        public DateTime ExecutionTime { get; set; }
        public int ExecutionDuration { get; set; }
        public string ExtraProperties { get; set; }
        public int Count { get; set; }
    }
}
