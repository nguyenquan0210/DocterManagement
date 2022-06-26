using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MasterData
{
    public class EthnicCreateRequest
    {
        [Display(Name = "Tên dân tộc")]
        public string Name { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "vị tí sắp xếp")]
        public int SortOrder { get; set; }
    }
}
