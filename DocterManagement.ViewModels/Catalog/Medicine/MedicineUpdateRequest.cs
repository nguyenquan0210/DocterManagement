using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Medicine
{
    public class MedicineUpdateRequest
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageText { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
