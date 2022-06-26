using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IDoctorApiClient
    {
        Task<ApiResult<List<DoctorVm>>> GetTopFavouriteDoctors();
        Task<ApiResult<DoctorVm>> GetById(Guid Id);
        Task<ApiResult<PatientVm>> GetByPatientId(Guid Id);
        Task<ApiResult<List<PatientVm>>> GetPatientProfile(string username);
        Task<ApiResult<bool>> UpdateInfo(UpdatePatientInfoRequest request);
        Task<ApiResult<Guid>> AddInfo(AddPatientInfoRequest request);
        Task<ApiResult<List<UserVm>>> GetAllUser(string? role);
    }
}
