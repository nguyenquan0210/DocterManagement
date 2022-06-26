using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Common
{
    public class ApiSuccessResult<T> : ApiResult<T>
    {
      
        public ApiSuccessResult(T data)
        {
            IsSuccessed = true;
            Data = data;
        }

        public ApiSuccessResult()
        {
            IsSuccessed = true;
        }
    }
}
