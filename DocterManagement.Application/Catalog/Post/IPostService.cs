using DoctorManagement.Data.Entities;
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
        Task<ApiResult<Posts>> Create(PostCreateRequest request);

        Task<ApiResult<Posts>> Update(PostUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<PostVm>>> GetAllPaging(GetPostPagingRequest request);

        Task<ApiResult<List<PostVm>>> GetAll();

        Task<ApiResult<PostVm>> GetById(Guid Id);

        Task<ApiResult<ImagesVm>> AddImage(ImageCreateRequest request);
    }
}
