using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class ScheduleSlotsVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Từ giờ ")]
        public TimeSpan FromTime { get; set; }
        [Display(Name = "Đến giờ")]
        public TimeSpan ToTime { get; set; }
        public int Type { get; set; }
    }
}
