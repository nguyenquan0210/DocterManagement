using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class ScheduleCreateRequest
    {
        [Display(Name = "Thời gian làm")]
        public TimeSpan FromTime { get; set; }
        [Display(Name = "Thời gian nghĩ")]
        public TimeSpan ToTime { get; set; }
        [Display(Name = "Số lượng")]
        public int Qty { get; set; }
        [Display(Name = "Từ ngày")]
        public DateTime FromDay { get; set; }
        [Display(Name = "Đến ngày")]
        public DateTime ToDay { get; set; }
        public string? Username { get; set; }
        [Display(Name = "Thứ")]
        public string WeekDay { get; set; }
    }
}
