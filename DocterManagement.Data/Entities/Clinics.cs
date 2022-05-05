using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Clinics
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImgLogo { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MapUrl { get; set; }
        public string Note { get; set; }
        public Status Status { get; set; }
        public Locations Locations { get; set; }
        public string No { get; set; }
        public Guid LocationId { get; set; }

        public List<ImageClinics> ImageClinics { get; set; }
        public List<Doctors> Doctors { get; set; }

    }
}
