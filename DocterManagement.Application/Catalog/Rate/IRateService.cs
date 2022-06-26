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
        Task<ApiResult<bool>> Create(RateCreateRequest request);

        Task<ApiResult<bool>> Update(RateUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<RatesVm>>> GetAllPaging(GetRatePagingRequest request);

        Task<ApiResult<List<RatesVm>>> GetAll();

        Task<ApiResult<RatesVm>> GetById(Guid Id);
    }
}
