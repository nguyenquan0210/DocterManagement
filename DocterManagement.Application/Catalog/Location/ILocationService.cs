using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Location
{
    public interface ILocationService
    {
        Task<ApiResult<bool>> Create(LocationCreateRequest request);

        Task<ApiResult<bool>> Update(LocationUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<LocationVm>>> GetAllPaging(GetLocationPagingRequest request);
        Task<ApiResult<List<LocationVm>>> GetListAllPaging(string? type);

        Task<ApiResult<List<LocationVm>>> GetAllSubDistrict(Guid districtId);
        Task<ApiResult<List<LocationVm>>> GetAllDistrict(Guid provinceId);
        Task<ApiResult<List<LocationVm>>> GetAllDistrictDaNangCity();
        Task<ApiResult<List<LocationVm>>> GetAllProvince();

        Task<ApiResult<LocationVm>> GetById(Guid Id);
    }
}
