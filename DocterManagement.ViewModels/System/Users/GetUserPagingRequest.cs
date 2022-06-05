using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
        public string? searchSpeciality { get; set; }

        public string? RoleName { get; set; }
        public Guid? SpecialityId { get; set; }
    }
}
