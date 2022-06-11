using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicVm
    {
        public Guid Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string ImgLogo { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public LocationVm LocationVm { get; set; }
        public Status Status { get; set; }
        public List<ImageClinicVm> Images { get; set; }
        public List<DoctorVm> DoctorVms { get; set; }
        public string FullAddress { get; set; }

        public DateTime CreatedAt { get; set; }
        public string MapUrl { get; set; }
        public string Note { get; set; }
        public int Rating { get; set; }
    }
}
