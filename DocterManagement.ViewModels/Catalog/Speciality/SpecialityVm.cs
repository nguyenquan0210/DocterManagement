using DoctorManagement.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Speciality
{
    public class SpecialityVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string No { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }
        public bool IsDeleted { get; set; }
    }
}
