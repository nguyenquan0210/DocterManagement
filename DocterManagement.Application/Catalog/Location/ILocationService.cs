using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Ward
{
    public interface ILocationService
    {
        Task<ApiResult<Locations>> Create(LocationCreateRequest request);

        Task<ApiResult<Locations>> Update(LocationUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<LocationVm>>> GetAllPaging(GetLocationPagingRequest request);
        Task<ApiResult<List<LocationVm>>> GetListAllPaging(string? type);

        Task<ApiResult<List<LocationVm>>> GetAllSubDistrict(Guid districtId);
        Task<ApiResult<List<LocationVm>>> GetAllDistrict(Guid provinceId);
        Task<ApiResult<List<LocationVm>>> GetAllProvince();

        Task<ApiResult<LocationVm>> GetById(Guid Id);
    }
}
