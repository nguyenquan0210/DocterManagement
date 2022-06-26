using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class GetAnnualServiceFeePagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
        public string? day { get; set; }
        public string? month { get; set; }
        public string? year { get; set; }
        public StatusAppointment? status { get; set; }
        public string? UserName { get; set; }
    }
}
