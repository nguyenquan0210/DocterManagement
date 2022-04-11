using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface ILocationApiClient
    {
        Task<ApiResult<PagedResult<LocationVm>>> GetLocationPagings(GetLocationPagingRequest request);
        Task<ApiResult<bool>> Update(LocationUpdateRequest request);
        Task<ApiResult<bool>> Create(LocationCreateRequest request);

        Task<int> Delete(Guid Id);

        Task<ApiResult<List<LocationVm>>> GetAllSubDistrict();
        Task<ApiResult<List<LocationVm>>> GetAllDistrict();
        Task<ApiResult<List<LocationVm>>> GetAllProvince();

        Task<ApiResult<LocationVm>> GetById(Guid Id);
    }
}
