using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.MedicalRecords
{
    public interface IMedicalRecordService
    {
        Task<ApiResult<MedicalRecord>> Create(MedicalRecordCreateRequest request);

        Task<ApiResult<MedicalRecord>> Update(MedicalRecordUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<MedicalRecordVm>>> GetAllPaging(GetMedicalRecordPagingRequest request);

        Task<ApiResult<List<MedicalRecordVm>>> GetAll();

        Task<ApiResult<MedicalRecordVm>> GetById(Guid Id);
    }
}
