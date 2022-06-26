using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Utilities.Exceptions
{
    public class DoctorManageException : Exception
    {
        public DoctorManageException()
        {
        }

        public DoctorManageException(string message)
            : base(message)
        {
        }

        public DoctorManageException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
