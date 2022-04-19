using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        Task<List<SelectListItem>> GetAllSubDistrict(Guid? subDistricyId);
        Task<List<SelectListItem>> GetAllDistrict(Guid? districyId);
        Task<List<SelectListItem>> GetAllProvince(Guid? provinceId);

        Task<ApiResult<LocationVm>> GetById(Guid Id);
    }
}
