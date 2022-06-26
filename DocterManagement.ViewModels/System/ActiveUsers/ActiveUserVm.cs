using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.ActiveUsers
{
    public class ActiveUserVm
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateActive { get; set; }
    }
}
