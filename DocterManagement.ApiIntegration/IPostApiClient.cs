using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IPostApiClient
    {
        Task<ApiResult<bool>> Create(PostCreateRequest request);

        Task<ApiResult<bool>> Update(PostUpdateRequest request);

        Task<int> DeleteDoctor(Guid Id);
        Task<int> DeleteAdmin(Guid Id);

        Task<ApiResult<PagedResult<PostVm>>> GetAllPaging(GetPostPagingRequest request);
        Task<ApiResult<PagedResult<PostVm>>> AdminGetAllPaging(GetPostPagingRequest request);

        Task<ApiResult<List<PostVm>>> GetAll();

        Task<ApiResult<PostVm>> GetById(Guid Id);

        Task<ApiResult<string>> AddImage(ImageCreateRequest request);
    }
}
