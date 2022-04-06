using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Ward;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Ward
{
    public interface IWardService
    {
        Task<ApiResult<Wards>> Create(WardCreateRequest request);

        Task<ApiResult<Wards>> Update(WardUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<WardVm>>> GetAllPaging(GetWardPagingRequest request);

        Task<ApiResult<List<WardVm>>> GetAll();

        Task<ApiResult<WardVm>> GetById(Guid Id);
    }
}
