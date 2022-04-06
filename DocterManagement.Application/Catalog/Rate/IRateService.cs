using DoctorManagement.Data.Entities;
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
        Task<ApiResult<Rates>> Create(RateCreateRequest request);

        Task<ApiResult<Rates>> Update(RateUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<RateVm>>> GetAllPaging(GetRatePagingRequest request);

        Task<ApiResult<List<RateVm>>> GetAll();

        Task<ApiResult<RateVm>> GetById(Guid Id);
    }
}
