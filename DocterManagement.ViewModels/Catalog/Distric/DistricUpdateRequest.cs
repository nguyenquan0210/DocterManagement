using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Distric
{
    public class DistricUpdateRequest
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

    }
}
