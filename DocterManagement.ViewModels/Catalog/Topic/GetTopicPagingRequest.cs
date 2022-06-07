using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Topic
{
    public class GetTopicPagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
    }
}
