using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.ScheduleDetailt;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.ScheduleDetailt
{
    public interface IScheduleDetailtService
    {
        Task<ApiResult<SchedulesDetailts>> Create(ScheduleDetailtCreateRequest request);

        Task<ApiResult<SchedulesDetailts>> Update(ScheduleDetailtUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<ScheduleDetailtVm>>> GetAllPaging(GetScheduleDetailtPagingRequest request);

        Task<ApiResult<List<ScheduleDetailtVm>>> GetAll();

        Task<ApiResult<ScheduleDetailtVm>> GetById(Guid Id);
    }
}
