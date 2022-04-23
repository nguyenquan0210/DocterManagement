﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Speciality
{
    public class SpecialityCreateRequest
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

    }
}
