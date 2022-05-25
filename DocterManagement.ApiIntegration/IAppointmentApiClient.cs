using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IAppointmentApiClient
    {
        Task<ApiResult<PagedResult<AppointmentVm>>> GetAppointmentPagings(GetAppointmentPagingRequest request);
        Task<ApiResult<bool>> Update(AppointmentUpdateRequest request);
        Task<ApiResult<string>> Create(AppointmentCreateRequest request);

        Task<int> Delete(Guid Id);

        Task<ApiResult<List<AppointmentVm>>> GetAllAppointment();

        Task<ApiResult<AppointmentVm>> GetById(Guid Id);
    }
}
