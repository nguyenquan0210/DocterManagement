using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IFormFile ImgLogo { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Guid LocationId { get; set; }
       
        [Required]
        public Guid DistrictId { get; set; }

        [Required]
        public IFormFileCollection ImgClinics { get; set; }
        public string MapUrl { get; set; }
        public string Note { get; set; }
    }
}
