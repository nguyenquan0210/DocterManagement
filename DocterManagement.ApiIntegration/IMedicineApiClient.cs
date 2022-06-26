using DoctorManagement.ViewModels.Catalog.Medicine;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IMedicineApiClient
    {
        Task<ApiResult<bool>> Create(MedicineCreateRequest request);

        Task<ApiResult<bool>> Update(MedicineUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<ApiResult<PagedResult<MedicineVm>>> GetAllPaging(GetMedicinePagingRequest request);

        Task<ApiResult<List<MedicineVm>>> GetAll(string UserName);

        Task<ApiResult<MedicineVm>> GetById(Guid Id);
    }
}
