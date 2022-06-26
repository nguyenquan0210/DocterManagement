using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Schedule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.SlotSchedule
{
    public class SlotScheduleVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Từ giờ")]
        public TimeSpan FromTime { get; set; }
        [Display(Name = "Đến giờ")]
        public TimeSpan ToTime { get; set; }
        [Display(Name = "Lịch khám")]
        public Guid ScheduleId { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Tình trạng đặt khám")]
        public bool IsBooked { get; set; }
        [Display(Name = "Lịch khám")]
        public ScheduleVm Schedule { get; set; }
    }
}
