using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }

        public string Body { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string ActionType { get; set; }
        public Guid? ActionId { get; set; }
        public string Type { get; set; }
        public bool IsDeleted { get; set; }
    }
}
