using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Speciality
{
    public class GetSpecialityVm
    {
        [Display(Name = "Mã khoa")]
        public Guid Id { get; set; }
        [Display(Name = "Tên khoa")]
        public string Title { get; set; }
    }
}
