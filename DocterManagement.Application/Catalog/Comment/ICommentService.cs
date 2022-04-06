using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Comment;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Comment
{
    public interface ICommentService
    {
        Task<ApiResult<CommentsPost>> Create(CommentCreateRequest request);

        Task<ApiResult<CommentsPost>> Update(CommentUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<CommentVm>>> GetAllPaging(GetCommentPagingRequest request);

        Task<ApiResult<List<CommentVm>>> GetAll();

        Task<ApiResult<CommentVm>> GetById(Guid Id);
    }
}
