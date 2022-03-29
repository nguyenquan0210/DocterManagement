using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Entities
{
    public class Clinics
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImgLogo { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Wards Wards { get; set; }
        public Guid WardId { get; set; }

        public List<ImageClinics> ImageClinics { get; set; }
        public List<Doctors> Doctors { get; set; }

    }
}
