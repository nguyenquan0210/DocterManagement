using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Entities
{
    public class ImageClinics
    {
        public Guid Id { get; set; }
        public string Img { get; set; }
        public int SortOrder { get; set; }
        public Clinics Clinics { get; set; }
        public Guid ClinicId    { get; set; }

    }
}
