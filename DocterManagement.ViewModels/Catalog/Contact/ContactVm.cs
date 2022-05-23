using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Contact
{
    public class ContactVm
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public string PhoneNumber { get; set; }
        public string YourMessage { get; set; }
        public DateTime CratedAt { get; set; }
    }
}
