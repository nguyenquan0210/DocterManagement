using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Post
{
    public class PostVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public int Views { get; set; }
        public DoctorVm Doctors { get; set; }
       
        public MainMenuVm Topic { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
    }
}
