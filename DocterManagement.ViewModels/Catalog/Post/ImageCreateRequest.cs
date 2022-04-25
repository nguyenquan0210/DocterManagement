using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class ImageCreateRequest
    {
        public IFormFile File { get; set; }
        public string? Watermark { get; set; }
    }
}
