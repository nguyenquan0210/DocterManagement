using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Clinic
{
    public interface IClinicService
    {
        Task<Guid> Create(ClinicCreateRequest request);

        Task<int> Update(ClinicUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<ClinicVm>> GetAllPaging(GetClinicPagingRequest request);

        Task<List<ClinicVm>> GetAll();

        Task<ClinicVm> GetById(Guid Id);
    }
}
