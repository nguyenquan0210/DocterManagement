using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class GetClinicVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Tên phòng khám")]
        public string Name { get; set; }
    }
}
