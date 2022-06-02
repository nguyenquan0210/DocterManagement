﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Service
{
    public class ServiceVm
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public string ServiceName { get; set; }
        public Guid DoctorId { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public List<ServiceDetailtVm> ServiceDetailts { get; set; }
    }
}