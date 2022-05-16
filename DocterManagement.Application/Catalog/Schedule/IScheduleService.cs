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
        Task<ApiResult<bool>> Create(ScheduleCreateRequest request);

        Task<ApiResult<bool>> Update(ScheduleUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<ScheduleVm>>> GetAllPaging(GetSchedulePagingRequest request);

        Task<ApiResult<List<ScheduleVm>>> GetAll();
        Task<ApiResult<List<DoctorScheduleClientsVm>>> GetScheduleDoctor(Guid DoctorId);

        Task<ApiResult<ScheduleVm>> GetById(Guid Id);
    }
}
