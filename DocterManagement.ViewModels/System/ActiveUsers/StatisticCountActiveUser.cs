using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.ActiveUsers
{
    public class StatisticCountActiveUser
    {
        public decimal countuserMonthNow { get; set; }
        public decimal countuserMonthBefor { get; set; }
        public string change { get; set; }
        public int percent { get; set; }
    }
}
