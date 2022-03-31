using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Speciality
{
    public interface ISpecialityService
    {
        Task<Guid> Create(SpecialityCreateRequest request);

        Task<int> Update(SpecialityUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<SpecialityVm>> GetAllPaging(GetSpecialityPagingRequest request);

        Task<List<SpecialityVm>> GetAll();

        Task<SpecialityVm> GetById(Guid Id);
    }
}
