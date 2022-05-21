using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
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
        Task<ApiResult<PatientVm>> GetByPatientId(Guid id);
        Task<ApiResult<bool>> UpdateInfo(UpdatePatientInfoRequest request);
        Task<ApiResult<bool>> AddInfo(AddPatientInfoRequest request);
        Task<ApiResult<List<PatientVm>>> GetPatientProfile(string username);
    }
}
