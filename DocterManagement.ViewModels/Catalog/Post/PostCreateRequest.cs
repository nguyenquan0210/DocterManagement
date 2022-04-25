﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class PostCreateRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid DoctorId { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
