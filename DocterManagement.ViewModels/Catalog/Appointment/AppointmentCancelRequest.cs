using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Appointment
{
    public class AppointmentCancelRequest
    {

        public Guid Id { get; set; }
        [Display(Name = "Lý do hủy")]
        public string CancelReason { get; set; }
        public string Checked { get; set; }
    }
}
