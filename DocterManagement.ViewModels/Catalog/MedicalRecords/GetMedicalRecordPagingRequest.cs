using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MedicalRecords
{
    public class GetMedicalRecordPagingRequest : PagingResultBase
    {
        public string? Keyword { get; set; }
    }
}
