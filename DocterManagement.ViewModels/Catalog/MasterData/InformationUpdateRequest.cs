using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MasterData
{
    public class InformationUpdateRequest
    {
        public Guid Id { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public IFormFile? Image { get; set; }
        public string Hotline { get; set; }
        public string TimeWorking { get; set; }
        public string FullAddress { get; set; }
    }
}
