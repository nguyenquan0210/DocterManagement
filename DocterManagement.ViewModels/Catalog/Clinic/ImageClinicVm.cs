using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ImageClinicVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }
        [Display(Name = "Vị trí sắp xếp")]
        public int SortOrder { get; set; }
    }
}
