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
        Task<Guid> Create(ScheduleCreateRequest request);

        Task<int> Update(ScheduleUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<ScheduleVm>> GetAllPaging(GetSchedulePagingRequest request);

        Task<List<ScheduleVm>> GetAll();

        Task<ScheduleVm> GetById(Guid Id);
    }
}
