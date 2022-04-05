﻿using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Post
{
    public class PostService : IPostService
    {
        private readonly DoctorManageDbContext _context;

        public PostService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(PostCreateRequest request)
        {
            var posts = new Posts()
            {
                Title = request.Title,
                Date = DateTime.Now,
                Description = request.Description,
                Status = Data.Enums.Status.Active,
                DoctorId = request.DoctorId
            };
            _context.Posts.Add(posts);
            await _context.SaveChangesAsync();
            return posts.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var posts = await _context.Posts.FindAsync(Id);
            int check = 0;
            if (posts == null) return check;
            if (posts.Status == Status.Active)
            {
                posts.Status = Status.InActive;
                check = 1;
            }
            else
            {
                _context.Posts.Remove(posts);
                check = 2;
            }
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<PostVm>> GetAll()
        {
            var query = _context.Posts.Where(x => x.Status == Status.Active);

            return await query.Select(x => new PostVm()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                Date = x.Date,
                DoctorId = x.DoctorId
            }).ToListAsync();
        }

        public async Task<PagedResult<PostVm>> GetAllPaging(GetPostPagingRequest request)
        {
            var query = from c in _context.Posts select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Title.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVm()
                {
                    Title = x.Title,
                    Description = x.Description,
                    Id = x.Id,
                    Status = x.Status,
                    DoctorId = x.DoctorId,
                    Date = x.Date

                }).ToListAsync();

            var pagedResult = new PagedResult<PostVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<PostVm> GetById(Guid Id)
        {
            var post = await _context.Posts.FindAsync(Id);
            if (post == null) throw new DoctorManageException($"Cannot find a Post with id: { Id}");
            var rs = new PostVm()
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Date = post.Date,
                DoctorId = post.DoctorId,
                Status = post.Status
            };

            return rs;
        }

        public async Task<int> Update(PostUpdateRequest request)
        {
            var post = await _context.Posts.FindAsync(request.Id);
            if (post == null) throw new DoctorManageException($"Cannot find a post with id: { request.Id}");
            post.Title = request.Title;
            post.DoctorId = request.DoctorId;
            post.Description = request.Description;
            post.Status = request.Status ? Status.Active : Status.InActive;

            return await _context.SaveChangesAsync();
        }
    }
}