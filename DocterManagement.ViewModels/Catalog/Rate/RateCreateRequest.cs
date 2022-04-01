using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Rate
{
    public class RateCreateRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Rating { get; set; }
        public Guid AppointmentId { get; set; }
    }
}
