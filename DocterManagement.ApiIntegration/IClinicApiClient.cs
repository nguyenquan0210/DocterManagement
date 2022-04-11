using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IClinicApiClient
    {
        Task<ApiResult<PagedResult<ClinicVm>>> GetClinicPagings(GetClinicPagingRequest request);
        Task<ApiResult<bool>> Update(ClinicUpdateRequest request);
        Task<ApiResult<bool>> Create(ClinicCreateRequest request);

        Task<int> Delete(Guid Id);

        Task<ApiResult<List<ClinicVm>>> GetMenu();

        Task<ApiResult<ClinicVm>> GetById(Guid Id);
    }
}
