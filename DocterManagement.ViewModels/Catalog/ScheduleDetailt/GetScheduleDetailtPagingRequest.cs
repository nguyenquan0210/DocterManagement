using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.ScheduleDetailt
{
    public class GetScheduleDetailtPagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
    }
}
