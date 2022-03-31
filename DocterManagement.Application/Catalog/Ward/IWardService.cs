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
        Task<Guid> Create(WardCreateRequest request);

        Task<int> Update(WardUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<WardVm>> GetAllPaging(GetWardPagingRequest request);

        Task<List<WardVm>> GetAll();

        Task<WardVm> GetById(Guid Id);
    }
}
