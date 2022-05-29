using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Statistic
{
    public class GetHistoryActivePagingRequest
    {
        public string? Keyword { get; set; }
        public string? day { get; set; }
        public string? month { get; set; }
        public string? year { get; set; }
    }
}
