using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class ImagesVm
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string OrgFileName { get; set; }
        public string OrgFileExtension { get; set; }
        public string FileUrl { get; set; }
        public string Container { get; set; }
    }
}
