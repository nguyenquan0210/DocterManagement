using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Schedule
{
    public class DoctorScheduleClientsVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime DateTime { get; set; }
        [Display(Name = "Số lượng còn lại")]
        public int AvailableQty { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid DoctorId { get; set; }
        [Display(Name = "Lịch đặt chỗ")]
        public List<ScheduleSlotsVm> ScheduleSlots { get; set; }
        [Display(Name = "Trạng")]
        public int CountTimeSpan { get; set; }
    }
}
