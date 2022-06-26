using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Contact
{
    public class GetContactPagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
    }
}
