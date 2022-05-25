using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Entities
{
    public class HistoryActives
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string User { get; set; }
        public int Qty { get; set; }
        public string Type { get; set; }
        public List<HistoryActiveDetailts> HistoryActiveDetailts { get; set; }
    }
}
