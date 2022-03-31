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
        Task<Guid> Create(MedicalRecordCreateRequest request);

        Task<int> Update(MedicalRecordUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<MedicalRecordVm>> GetAllPaging(GetMedicalRecordPagingRequest request);

        Task<List<MedicalRecordVm>> GetAll();

        Task<MedicalRecordVm> GetById(Guid Id);
    }
}
