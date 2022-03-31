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
        Task<Guid> Create(CommentCreateRequest request);

        Task<int> Update(CommentUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<CommentVm>> GetAllPaging(GetCommentPagingRequest request);

        Task<List<CommentVm>> GetAll();

        Task<CommentVm> GetById(Guid Id);
    }
}
