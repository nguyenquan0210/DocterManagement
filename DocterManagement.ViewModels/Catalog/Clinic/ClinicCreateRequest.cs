﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class ClinicCreateRequest
    {
        public string Name { get; set; }
        public string ImgLogo { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Guid WardId { get; set; }
    }
}