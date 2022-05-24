using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Statistic
{
    public class HistoryActiveVm
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string User { get; set; }
        public int Qty { get; set; }
        public string Type { get; set; }
        public List<HistoryActiveDetailtVm> HistoryActiveDetailts { get; set; }
    }
}
