using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class MedicineCreate
    {
        public Guid MedicineId { get; set; }
        [Display(Name = "Số lượng")]
        public int Qty { get; set; }
        [Display(Name = "Cách dùng")]
        public string Use { get; set; }
        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; }
        [Display(Name = "Tên thuốc")]
        public string? Name { get; set; }
        [Display(Name = "Giá")]
        public decimal? Price { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal? TotalAmount { get; set; }
        [Display(Name = "Tổng tiền")]
        public string? TotalAmountString { get; set; }
        [Display(Name = "Buổi sáng")]
        public int? Morning { get; set; }
        [Display(Name = "Buổi trưa")]
        public int? Noon { get; set; }
        [Display(Name = "Buổi chiều")]
        public int? Afternoon { get; set; }
        [Display(Name = "Buổi tối")]
        public int? Night { get; set; }
        [Display(Name = "Lịch hẹn")]
        public Guid AppointmentId { get; set; }

    }
}
