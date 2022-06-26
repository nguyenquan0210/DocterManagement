using DoctorManagement.ViewModels.Catalog.Contact;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Contact
{
    public interface IContactService
    {
        Task<ApiResult<bool>> CreateContact(ContactCreateRequest request);

        Task<ApiResult<bool>> UpdateContact(ContactUpdateRequest request);

        Task<ApiResult<int>> DeleteContact(Guid Id);

        Task<ApiResult<PagedResult<ContactVm>>> GetAllPagingContact(GetContactPagingRequest request);

        Task<ApiResult<List<ContactVm>>> GetAllContact();

        Task<ApiResult<ContactVm>> GetByIdContact(Guid Id);
    }
}
