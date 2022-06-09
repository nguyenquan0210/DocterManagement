using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Post
{
    public class PostService : IPostService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private const string POSTS_FEATURE_CONTENT_FOLDER_NAME = "posts-feature-content";
        private const string POSTS_CONTENT_FOLDER_NAME = "posts-content";
        private const string MASTERDATA_CONTENT_FOLDER_NAME = "masterData-content";
        public PostService(DoctorManageDbContext context,
            IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<ApiResult<string>> AddImage(ImageCreateRequest request)
        {
            
            if (request.File != null)
            {
                var img = POSTS_FEATURE_CONTENT_FOLDER_NAME + "/"+ await this.SaveFileFearure(request.File);
                return new ApiSuccessResult<string>(img);
            }

            await _context.SaveChangesAsync();
            return new ApiErrorResult<string>();
        }
        private async Task<string> SaveFile(IFormFile? file)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var orgFileExtension = Path.GetExtension(originalFileName);
            var guid = Guid.NewGuid();
            var fileName = $"{guid}{orgFileExtension}";
            await _storageService.SaveFileImgAsync(file.OpenReadStream(), fileName, POSTS_CONTENT_FOLDER_NAME);
            return fileName;
        }
        private async Task<string> SaveFileFearure(IFormFile? file)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var orgFileExtension = Path.GetExtension(originalFileName);
            var guid = Guid.NewGuid();
            var fileName = $"{guid}{orgFileExtension}";
            await _storageService.SaveFileAsyncs(file.OpenReadStream(), fileName, POSTS_FEATURE_CONTENT_FOLDER_NAME);
            return fileName;
        }
        public async Task<ApiResult<bool>> Create(PostCreateRequest request)
        {
            var posts = new Posts()
            {
                Title = request.Title,
                CreatedAt = DateTime.Now,
                Description = WebUtility.HtmlDecode(request.Description),
                Status = Status.Active,
                DoctorId = request.DoctorId,
                Image = await this.SaveFile(request.ImageFile),
                TopicId = request.TopicId,
                Content = WebUtility.HtmlDecode(request.Content),
                Views = 0,
            };
            _context.Posts.Add(posts);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Tạo bài viết không thành công!");
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var posts = await _context.Posts.FindAsync(Id);
            int check = 0;
            if (posts == null) return new ApiSuccessResult<int>(check);
            if (posts.Status == Status.Active)
            {
                posts.Status = Status.InActive;
                check = 1;
            }
            else
            {
                posts.Status = Status.NotActivate;
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<PostVm>>> GetAll()
        {
            var query = from c in _context.Posts select c;

            var rs = await query.Select(x => new PostVm()
            {
                Title = x.Title,
                Description = x.Description,
                Content = x.Content,
                Views = x.Views,
                Id = x.Id,
                Status = x.Status,
                Image = POSTS_CONTENT_FOLDER_NAME + "/" + x.Image,
                Topic = new MainMenuVm()
                {
                    Id = x.TopicId,
                    Description = x.MainMenus.Description,
                    Title = x.MainMenus.Title,
                    Type = x.MainMenus.Type,
                    Image = MASTERDATA_CONTENT_FOLDER_NAME + "/" + x.MainMenus.Image,
                    Name = x.MainMenus.Name,

                },
                Doctors = new DoctorVm()
                {
                    UserId = x.DoctorId,
                    FirstName = x.Doctors.FirstName,
                    LastName = x.Doctors.LastName,
                    Img = x.Doctors.Img,
                    GetSpecialities = x.Doctors.ServicesSpecialities.Select(x => new GetSpecialityVm()
                    {
                        Id = x.Specialities.Id,
                        Title = x.Specialities.Title,
                    }).ToList()

                },
                CreatedAt = x.CreatedAt,

            }).ToListAsync();
            return new ApiSuccessResult<List<PostVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<PostVm>>> GetAllPaging(GetPostPagingRequest request)
        {
            var query = from c in _context.Posts select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Title.Contains(request.Keyword)|| x.Doctors.FirstName.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.Usename))
            {
                query = query.Where(x => x.Doctors.AppUsers.UserName == request.Usename);
            }
            if (request.TopicId != null)
            {
                query = query.Where(x => x.TopicId == request.TopicId|| x.DoctorId == request.TopicId);
            }
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x=>x.CreatedAt).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVm()
                {
                    Title = x.Title,
                    Description = x.Description,
                    Content = x.Content,
                    Views = x.Views,
                    Id = x.Id,
                    Status = x.Status,
                    Image = POSTS_CONTENT_FOLDER_NAME + "/" + x.Image,
                    Topic = new MainMenuVm()
                    {
                        Id = x.TopicId,
                        Description = x.MainMenus.Description,
                        Title = x.MainMenus.Title,
                        Type = x.MainMenus.Type,
                        Image = MASTERDATA_CONTENT_FOLDER_NAME + "/"+ x.MainMenus.Image,
                        Name = x.MainMenus.Name,

                    },
                    Doctors = new DoctorVm()
                    {
                        UserId = x.DoctorId,
                        FirstName = x.Doctors.FirstName,
                        LastName = x.Doctors.LastName,
                        Img = x.Doctors.Img,
                        GetSpecialities = x.Doctors.ServicesSpecialities.Select(x=> new GetSpecialityVm()
                        {
                            Id = x.Specialities.Id,
                            Title = x.Specialities.Title,
                        }).ToList()
                        
                    },
                    CreatedAt = x.CreatedAt,
                    

                }).ToListAsync();

            var pagedResult = new PagedResult<PostVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<PostVm>>(pagedResult);
        }

        public async Task<ApiResult<PostVm>> GetById(Guid Id)
        {
            var x = await _context.Posts.FindAsync(Id);
            if (x == null) return new ApiErrorResult<PostVm>("Bài viết không được xác nhân!");
            var menus = await _context.MainMenus.FindAsync(x.TopicId);
            var doctors = await _context.Doctors.FindAsync(x.DoctorId);
            var GetSpecialities =  _context.ServicesSpecialities;
            var rs = new PostVm()
            {
                Title = x.Title,
                Description = x.Description,
                Content = x.Content,
                Views = x.Views,
                Id = x.Id,
                Status = x.Status,
                Image = POSTS_CONTENT_FOLDER_NAME + "/" + x.Image,
                Topic = new MainMenuVm()
                {
                    Id = menus.Id,
                    Description = menus.Description,
                    Title = menus.Title,
                    Type = menus.Type,
                    Image = menus.Image,
                    Name = menus.Name,

                },
                Doctors = new DoctorVm()
                {
                    UserId = doctors.UserId,
                    FirstName = doctors.FirstName,
                    LastName = doctors.LastName,
                    Img = doctors.Img,
                    GetSpecialities = GetSpecialities.Where(x=>x.DoctorId == doctors.UserId).Select(x => new GetSpecialityVm()
                    {
                        Id = x.Specialities.Id,
                        Title = x.Specialities.Title,
                    }).ToList()

                },
                CreatedAt = x.CreatedAt,
            };

            return new ApiSuccessResult<PostVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(PostUpdateRequest request)
        {
            var posts = await _context.Posts.FindAsync(request.Id);
            if (posts == null) return new ApiErrorResult<bool>("Bài viết không được xác nhân!");
            posts.Title = request.Title;
            posts.Description = WebUtility.HtmlDecode(request.Description);
            posts.Content = WebUtility.HtmlDecode(request.Content);
            posts.TopicId = request.TopicId;
            posts.Status = request.Status ? Status.Active : Status.InActive;
            if (request.ImageFile != null)
            {
                if (posts.Image != null && posts.Image != "default") await _storageService.DeleteFileAsyncs(posts.Image, POSTS_CONTENT_FOLDER_NAME);
                posts.Image = await SaveFile(request.ImageFile);
            }

          
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Cập nhật bài viết không thành công!");
        }
    }
}
