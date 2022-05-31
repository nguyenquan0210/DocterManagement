using DoctorManagement.ViewModels.Catalog.Medicine;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Medicine
{
    public interface IMedicineService
    {
        Task<ApiResult<bool>> Create(MedicineCreateRequest request);

        Task<ApiResult<bool>> Update(MedicineUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<MedicineVm>>> GetAllPaging(GetMedicinePagingRequest request);

        Task<ApiResult<List<MedicineVm>>> GetAll(Guid ParentId);

        Task<ApiResult<MedicineVm>> GetById(Guid Id);
    }
}
