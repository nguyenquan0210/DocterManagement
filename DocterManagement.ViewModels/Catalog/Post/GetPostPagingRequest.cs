using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class GetPostPagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
        public string? Usename { get; set; }
    }
}
