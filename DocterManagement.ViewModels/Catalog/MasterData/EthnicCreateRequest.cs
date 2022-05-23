using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MasterData
{
    public class EthnicCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
    }
}
