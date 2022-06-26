using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Clinic
{
    public class GetClinicPagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
        public Guid? WardId { get; set; }
        public Guid? DistricId { get; set; }
    }
}
