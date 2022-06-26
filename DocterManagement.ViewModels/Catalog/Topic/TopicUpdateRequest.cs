using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Topic
{
    public class TopicUpdateRequest
    {
        public Guid Id { get; set; }
        public string? ImageText { get; set; }
        public IFormFile? Image { get; set; }
        public string Titile { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
