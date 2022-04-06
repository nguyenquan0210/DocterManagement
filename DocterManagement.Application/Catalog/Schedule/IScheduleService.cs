using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Schedule
{
    public interface IScheduleService
    {
        Task<ApiResult<Schedules>> Create(ScheduleCreateRequest request);

        Task<ApiResult<Schedules>> Update(ScheduleUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<ScheduleVm>>> GetAllPaging(GetSchedulePagingRequest request);

        Task<ApiResult<List<ScheduleVm>>> GetAll();

        Task<ApiResult<ScheduleVm>> GetById(Guid Id);
    }
}
