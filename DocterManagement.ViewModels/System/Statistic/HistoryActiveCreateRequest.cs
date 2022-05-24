using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Statistic
{
    public class HistoryActiveCreateRequest
    {
        public string User { get; set; }
        public string Type { get; set; }
        public string MethodName { get; set; }
        public string ServiceName { get; set; }
        public string Parameters { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string ExtraProperties { get; set; }
    }
}
