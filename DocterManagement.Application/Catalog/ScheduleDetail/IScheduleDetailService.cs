using DoctorManagement.ViewModels.Catalog.ScheduleDetail;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.ScheduleDetail
{
    public interface IScheduleDetailService
    {
        Task<Guid> Create(ScheduleDetailCreateRequest request);

        Task<int> Update(ScheduleDetailUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<ScheduleDetailVm>> GetAllPaging(GetScheduleDetailPagingRequest request);

        Task<List<ScheduleDetailVm>> GetAll();

        Task<ScheduleDetailVm> GetById(Guid Id);
    }
}
