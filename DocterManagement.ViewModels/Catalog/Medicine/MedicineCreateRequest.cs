using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Medicine
{
    public class MedicineCreateRequest
    {
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public decimal Price { get; set; }
    }
}
