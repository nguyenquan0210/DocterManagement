using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.SlotSchedule
{
    public interface ISlotScheduleService
    {
        Task<ApiResult<bool>> Create(SlotScheduleCreateRequest request);

        Task<ApiResult<bool>> Update(SlotScheduleUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<SlotScheduleVm>>> GetAllPaging(GetSlotSchedulePagingRequest request);

        Task<ApiResult<List<SlotScheduleVm>>> GetAll();

        Task<ApiResult<SlotScheduleVm>> GetById(Guid Id);
    }
}
