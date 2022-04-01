using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Post
{
    public interface IPostService
    {
        Task<Guid> Create(PostCreateRequest request);

        Task<int> Update(PostUpdateRequest request);
            
        Task<int> Delete(Guid Id);

        Task<PagedResult<PostVm>> GetAllPaging(GetPostPagingRequest request);

        Task<List<PostVm>> GetAll();

        Task<PostVm> GetById(Guid Id);
    }
}
