using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Distric;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Distric
{
    public interface IDistricService
    {
        Task<ApiResult<Districs>> Create(DistricCreateRequest request);

        Task<ApiResult<Districs>> Update(DistricUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<DistricVm>>> GetAllPaging(GetDistricPagingRequest request);

        Task<ApiResult<List<DistricVm>>> GetAll();

        Task<ApiResult<DistricVm>> GetById(Guid Id);
    }
}
