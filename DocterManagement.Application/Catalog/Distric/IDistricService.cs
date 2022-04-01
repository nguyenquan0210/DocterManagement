using DoctorManagement.ViewModels.Catalog.Distric;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Distric
{
    public interface IDistricService
    {
        Task<Guid> Create(DistricCreateRequest request);

        Task<int> Update(DistricUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<DistricVm>> GetAllPaging(GetDistricPagingRequest request);

        Task<List<DistricVm>> GetAll();

        Task<DistricVm> GetById(Guid Id);
    }
}
