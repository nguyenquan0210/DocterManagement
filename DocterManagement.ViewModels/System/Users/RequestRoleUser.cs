using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class RequestRoleUser
    {
        public Guid IdUser { get; set; }
        public Guid IdRole { get; set; }

        public string NameRole { get; set; }
    }
}
