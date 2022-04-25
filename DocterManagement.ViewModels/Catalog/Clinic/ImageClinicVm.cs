using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ImageClinicVm
    {
        public Guid Id { get; set; }
        public string Img { get; set; }
        public int SortOrder { get; set; }
    }
}
