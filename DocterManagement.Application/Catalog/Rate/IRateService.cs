using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Rate
{
    public interface IRateService
    {
        Task<Guid> Create(RateCreateRequest request);

        Task<int> Update(RateUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<RateVm>> GetAllPaging(GetRatePagingRequest request);

        Task<List<RateVm>> GetAll();

        Task<RateVm> GetById(Guid Id);
    }
}
