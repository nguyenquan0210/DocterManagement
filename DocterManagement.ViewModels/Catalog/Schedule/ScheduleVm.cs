using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class ScheduleVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Từ giờ")]
        public TimeSpan FromTime { get; set; }
        [Display(Name = "Đến giờ")]
        public TimeSpan ToTime { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Ngày khám")]
        public DateTime CheckInDate { get; set; }
        [Display(Name = "Số lương")]
        public int Qty { get; set; }
        [Display(Name = "Số lương đã đặt")]
        public int BookedQty { get; set; }
        [Display(Name = "Số lượng còn lại")]
        public int AvailableQty { get; set; }
        [Display(Name = "bác sĩ")]
        public Guid DoctorId { get; set; }
        [Display(Name = "Chổ đặt khám")]
        public List<SlotScheduleVm> ScheduleDetailts { get; set; }

    }
}
