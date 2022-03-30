using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Speciality
{
    public class SpecialityUpdateRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }
        public bool Status { get; set; }
    }
}
