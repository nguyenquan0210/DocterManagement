using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class ScheduleUpdateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Từ giờ")]
        public TimeSpan FromTime { get; set; }
        [Display(Name = "Đến giờ")]
        public TimeSpan ToTime { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }
        [Display(Name = "Số lượng")]
        public int Qty { get; set; }
        [Display(Name = "Số lượng đã đặt")]
        public int? BookedQty { get; set; }
        [Display(Name = "Số lượng còn lại")]
        public int? AvailableQty { get; set; }
        [Display(Name = "Ngày khám")]
        public string? CheckInDate { get; set; }
    }
}
