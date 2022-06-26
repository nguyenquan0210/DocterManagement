using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class AnnualServiceFeePaymentRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Số tiền đã nộp")]
        public decimal TuitionPaidFreeNumBer { get; set; }
        [Display(Name = "Số tiền đã nộp bằng chữ viết")]
        public string TuitionPaidFreeText { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        [Display(Name = "Số tiền phải nộp")]
        public decimal? NeedToPay { get; set; }
        [Display(Name = "Số tiền còn dư")]
        public decimal? Contingency { get; set; }
    }
}
