using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class AnnualServiceFeeVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Trạng thái")]
        public StatusAppointment Status { get; set; }
        [Display(Name = "Số tiền phải nộp")]
        public decimal NeedToPay { get; set; }
        [Display(Name = "Số tiền đã nộp")]
        public decimal TuitionPaidFreeNumBer { get; set; }
        [Display(Name = "Số tiền đã nộp bằng chữ")]
        public string TuitionPaidFreeText { get; set; }
        [Display(Name = "Số tiền còn dư")]
        public decimal Contingency { get; set; }
        [Display(Name = "Số tài khoản ngân hàng")]
        public string AccountBank { get; set; }
        [Display(Name = "Hình thức nộp")]
        public string Type { get; set; }
        [Display(Name = "Hình ảnh chứng minh")]
        public string Image { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
        [Display(Name = "Mã giao dịch")]
        public string TransactionCode { get; set; }
        [Display(Name = "Ngày nộp")]
        public DateTime PaidDate { get; set; }

        [Display(Name = "Bác sĩ")]
        public DoctorVm Doctor { get; set; }
        [Display(Name = "Mã nộp phí")]
        public string No { get; set; }
        [Display(Name = "Thông tin website")]
        public InformationVm Information { get; set; }
        [Display(Name = "Lý do hủy")]
        public string CancelReason { get; set; }
        [Display(Name = "Số tiền gốc")]
        public decimal InitialAmount { get; set; }
    }
}
