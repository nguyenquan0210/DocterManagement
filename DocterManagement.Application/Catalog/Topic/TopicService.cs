using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Topic;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Topic
{
    public class TopicService : ITopicService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private const string TOPIC_CONTENT_FOLDER_NAME = "topic-content";
        public TopicService(DoctorManageDbContext context,
            IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<ApiResult<bool>> Create(TopicCreateRequest request)
        {
            var Topic = new Topics()
            {
                Description = request.Description,
                IsDeleted = false,
                Image = await SaveFile(request.Image, TOPIC_CONTENT_FOLDER_NAME),
                CreatedAt = DateTime.Now,
                Titile = request.Titile,

            };
            _context.Topics.Add(Topic);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Tạo Chủ đề không thành công!");
        }
        private async Task<string> SaveFile(IFormFile? file, string folderName)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsyncs(file.OpenReadStream(), fileName, folderName);
            return fileName;
        }
        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var Topics = await _context.Topics.FindAsync(Id);
            int check = 0;
            if (Topics == null) return new ApiSuccessResult<int>(check);

            Topics.IsDeleted = true;
            check = 2;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<TopicVm>>> GetAll()
        {
            var query = _context.Topics.Where(x => x.IsDeleted == false);

            var rs = await query.Select(x => new TopicVm()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                IsDeleted = x.IsDeleted,
                Titile = x.Titile,
                Description = x.Description,
                Image = TOPIC_CONTENT_FOLDER_NAME + "/" + x.Image,
            }).ToListAsync();
            return new ApiSuccessResult<List<TopicVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<TopicVm>>> GetAllPaging(GetTopicPagingRequest request)
        {
           
            var query = from m in _context.Topics
                        select m;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Titile.Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new TopicVm()
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    IsDeleted = x.IsDeleted,
                    Titile = x.Titile,
                    Description = x.Description,
                    Image = TOPIC_CONTENT_FOLDER_NAME + "/" + x.Image,
                }).ToListAsync();

            var pagedResult = new PagedResult<TopicVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<TopicVm>>(pagedResult);
        }

        public async Task<ApiResult<TopicVm>> GetById(Guid Id)
        {
            var x = await _context.Topics.FindAsync(Id);
            if (x == null) return new ApiErrorResult<TopicVm>("Chủ đề không được xác nhận!");
            var rs = new TopicVm()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                IsDeleted = x.IsDeleted,
                Titile = x.Titile,
                Description = x.Description,
                Image = TOPIC_CONTENT_FOLDER_NAME + "/" + x.Image,
            };
            return new ApiSuccessResult<TopicVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(TopicUpdateRequest request)
        {
            var Topics = await _context.Topics.FindAsync(request.Id);
            if (Topics == null) return new ApiErrorResult<bool>("Chủ đề không được xác nhận!");

            Topics.Description = request.Description;
            Topics.Titile = request.Titile;
            Topics.IsDeleted = request.IsDeleted;
            if (request.Image != null)
            {
                if (Topics.Image != null && Topics.Image != "default") await _storageService.DeleteFileAsyncs(Topics.Image, TOPIC_CONTENT_FOLDER_NAME);
                Topics.Image = await SaveFile(request.Image, TOPIC_CONTENT_FOLDER_NAME);
            }

            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Cập nhật Chủ đề không thành công!");
        }
    }
}
