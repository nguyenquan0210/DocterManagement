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
        Task<Guid> Create(ScheduleDetailtCreateRequest request);

        Task<int> Update(ScheduleDetailtUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<ScheduleDetailtVm>> GetAllPaging(GetScheduleDetailtPagingRequest request);

        Task<List<ScheduleDetailtVm>> GetAll();

        Task<ScheduleDetailtVm> GetById(Guid Id);
    }
}
