using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Ward
{
    public class WardCreateRequest
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public Guid DisticId { get; set; }
    }
}
