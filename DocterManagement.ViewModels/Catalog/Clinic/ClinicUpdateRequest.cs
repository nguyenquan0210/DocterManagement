using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Location;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicUpdateRequest
    {
        [Required]
        public string Name { get; set; }
     
        public IFormFile? ImgLogo { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Guid LocationId { get; set; }

        [Required]
        public Guid DistrictId { get; set; }

        public IFormFileCollection? ImgClinics { get; set; }
        public Guid Id { get; set; }
        public Status Status { get; set; }
    }
}
