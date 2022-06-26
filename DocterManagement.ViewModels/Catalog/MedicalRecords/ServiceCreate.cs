using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class ServiceCreate
    {
        public Guid ServiceId { get; set; }
        [Display(Name = "Số lượng")]
        public int Qty { get; set; }
        [Display(Name = "Tên dịch vụ")]
        public string? Name { get; set; }
        [Display(Name = "Giá tiền")]
        public decimal? Price { get; set; }
        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; }
        public Guid AppointmentId { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal? TotalAmount { get; set; }
        [Display(Name = "Tổng tiền")]
        public string? TotalAmountString { get; set; }
    }
}
