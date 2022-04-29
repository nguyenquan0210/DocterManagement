using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IScheduleApiClient
    {
        Task<ApiResult<PagedResult<ScheduleVm>>> GetSchedulePagings(GetSchedulePagingRequest request);
        Task<ApiResult<bool>> Update(ScheduleUpdateRequest request);
        Task<ApiResult<bool>> Create(ScheduleCreateRequest request);

        Task<int> Delete(Guid Id);

        Task<ApiResult<List<ScheduleVm>>> GetAllSchedule();

        Task<ApiResult<ScheduleVm>> GetById(Guid Id);
    }
}
