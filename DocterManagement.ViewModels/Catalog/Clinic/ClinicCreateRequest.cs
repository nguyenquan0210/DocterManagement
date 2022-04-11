using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicCreateRequest
    {
        public string Name { get; set; }
        public IFormFile ImgLogo { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Guid LocationId { get; set; }
        public List<IFormFile> ImgClinic { get; set; }
    }
}
