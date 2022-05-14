using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.Doctor
{
    public interface IDoctorService
    {
        Task<ApiResult<List<DoctorVm>>> GetTopFavouriteDoctors();
        Task<ApiResult<DoctorVm>> GetById(Guid id);
    }
}
