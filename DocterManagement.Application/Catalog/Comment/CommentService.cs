using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Comment;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Comment
{
    public class CommentService : ICommentService
    {
        private readonly DoctorManageDbContext _context;

        public CommentService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(CommentCreateRequest request)
        {
            var comments = new CommentsPost()
            {
                CheckComentId = request.CheckComentId,
                Date = DateTime.Now,
                Description = request.Description,
                CheckLevel = request.CheckLevel,
                UserId = request.UserId,
                PostId = request.PostId
            };
            _context.CommentsPost.Add(comments);
            await _context.SaveChangesAsync();
            return comments.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var comments = await _context.CommentsPost.FindAsync(Id);
            int check = 0;
            if (comments == null) return check;
            _context.CommentsPost.Remove(comments);
            check = 2;
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<CommentVm>> GetAll()
        {
            var query = _context.CommentsPost;

            return await query.Select(x => new CommentVm()
            {
                Id = x.Id,
                Date = x.Date,
                Description = x.Description,
                UserId = x.UserId,
                PostId = x.PostId,
                CheckLevel = x.CheckLevel,
                CheckComentId = x.CheckComentId
            }).ToListAsync();
        }

        public async Task<PagedResult<CommentVm>> GetAllPaging(GetCommentPagingRequest request)
        {
            var query = from c in _context.CommentsPost select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Description.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CommentVm()
                {
                    Date = x.Date,
                    Description = x.Description,
                    Id = x.Id,
                    UserId = x.UserId,
                    PostId = x.PostId

                }).ToListAsync();

            var pagedResult = new PagedResult<CommentVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<CommentVm> GetById(Guid Id)
        {
            var comments = await _context.CommentsPost.FindAsync(Id);
            if (comments == null) throw new DoctorManageException($"Cannot find a Comment with id: { Id}");
            var rs = new CommentVm()
            {
                Id = comments.Id,
                Date = comments.Date,
                Description = comments.Description,
                CheckLevel = comments.CheckLevel,
                CheckComentId = comments.CheckComentId,
                UserId= comments.UserId,
                PostId = comments.PostId
            };
            return rs;
        }

        public async Task<int> Update(CommentUpdateRequest request)
        {
            var comments = await _context.CommentsPost.FindAsync(request.Id);
            if (comments == null) throw new DoctorManageException($"Cannot find a Comment with id: { request.Id}");
            comments.Description = request.Description;
            return await _context.SaveChangesAsync();
        }
    }
}
