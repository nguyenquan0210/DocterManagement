using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class AnnualServiceFeePaymentDoctorRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Số tiền đã nộp")]
        public decimal TuitionPaidFreeNumBer { get; set; }
        [Display(Name = "Số tiền đã nộp bằng chữ")]
        public string TuitionPaidFreeText { get; set; }
        [Display(Name = "Số tài khoản ngân hàng")]
        public string AccountBank { get; set; }
        [Display(Name = "Tên ngân hàng")]
        public string BankName { get; set; }
        [Display(Name = "Hình ảnh giao dịch")]
        public IFormFile Image { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        [Display(Name = "Mã giao dịch")]
        public string TransactionCode { get; set; }

        [Display(Name = "Số tiền phải nộp")]
        public decimal? NeedToPay { get; set; }
        [Display(Name = "Số tiền còn dư")]
        public decimal? Contingency { get; set; }
    }
}
