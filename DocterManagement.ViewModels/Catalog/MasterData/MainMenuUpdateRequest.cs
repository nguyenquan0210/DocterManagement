using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MasterData
{
    public class MainMenuUpdateRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageHiden { get; set; }
        public IFormFile? Image { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public Guid ParentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
