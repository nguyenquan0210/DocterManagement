using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class AnnualServiceFeePaymentRequest
    {
        public Guid Id { get; set; }
        public decimal TuitionPaidFreeNumBer { get; set; }
        public string TuitionPaidFreeText { get; set; }
        public string Note { get; set; }
    }
}
