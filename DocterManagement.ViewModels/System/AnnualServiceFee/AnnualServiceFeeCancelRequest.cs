using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class AnnualServiceFeeCancelRequest
    {
        public Guid Id { get; set; }
        public string CancelReason { get; set; }
    }
    
}
