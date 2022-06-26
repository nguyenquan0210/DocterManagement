using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Rate
{
    public class RatesVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Ngày tao")]
        public DateTime Date { get; set; }
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        [Display(Name = "Nội dung")]
        public string Description { get; set; }
        [Display(Name = "Rate")]
        public int Rating { get; set; }
        [Display(Name = "Lịch hẹn")]
        public Guid AppointmentId { get; set; }
    }
}
