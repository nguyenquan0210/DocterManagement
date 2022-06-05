using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Speciality
{
    public class SpecialityService : ISpecialityService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private const string SPECIALITY_CONTENT_FOLDER_NAME = "speciality-content";
        public SpecialityService(DoctorManageDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<ApiResult<bool>> Create(SpecialityCreateRequest request)
        {
            string year = DateTime.Now.ToString("yy");
            int count = await _context.Specialities.Where(x => x.No.Contains("SP-" + year)).CountAsync();
            string str = "";
            if (count < 9) str = "SP-" + DateTime.Now.ToString("yy") + "-00" + (count + 1);
            else if (count < 99) str = "SP-" + DateTime.Now.ToString("yy") + "-0" + (count + 1);
            else if (count < 999) str = "SP-" + DateTime.Now.ToString("yy") + "-" + (count + 1);
            var specialities = new Specialities()
            {
                Title = request.Title,
                SortOrder = count + 1,
                Description = request.Description,
                IsDeleted = false,
                No = str,
                Img = await SaveFile(request.Img, SPECIALITY_CONTENT_FOLDER_NAME),
            };
            _context.Specialities.Add(specialities);
            var rs = await _context.SaveChangesAsync();
            if(rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Tạo chuyên khoa không thành công!");
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
            var speciality = await _context.Specialities.FindAsync(Id);
            int check = 0;
            if (speciality == null) return new ApiSuccessResult<int>(check);
            if (speciality.IsDeleted = false)
            {
                speciality.IsDeleted = true;
                check = 1;
            }
            else
            {
                _context.Specialities.Remove(speciality);
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<SpecialityVm>>> GetAllSpeciality()
        {
            var query = _context.Specialities.Where(x => x.IsDeleted == false);

            var rs = await query.Select(x => new SpecialityVm()
            {
                Id = x.Id,
                Title = x.Title,
                SortOrder = x.SortOrder,
                IsDeleted = x.IsDeleted,
                No = x.No,
                Description = x.Description,
                Img = SPECIALITY_CONTENT_FOLDER_NAME + "/" + x.Img,
            }).ToListAsync();
            return new ApiSuccessResult<List<SpecialityVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<SpecialityVm>>> GetAllPaging(GetSpecialityPagingRequest request)
        {
            var query = from c in _context.Specialities select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Title.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SpecialityVm()
                {
                    Title = x.Title,
                    SortOrder = x.SortOrder,
                    Id = x.Id,
                    IsDeleted = x.IsDeleted,
                    No = x.No,
                    Description = x.Description,
                    Img = SPECIALITY_CONTENT_FOLDER_NAME + "/" + x.Img,
                }).ToListAsync();

            var pagedResult = new PagedResult<SpecialityVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<SpecialityVm>>(pagedResult);
        }

        public async Task<ApiResult<SpecialityVm>> GetById(Guid Id)
        {
            var speciality = await _context.Specialities.FindAsync(Id);
            if (speciality == null) return new ApiErrorResult<SpecialityVm>("Chuyên khoa không được xác nhận!");
            var rs = new SpecialityVm()
            {
                Id = speciality.Id,
                Title = speciality.Title,
                SortOrder = speciality.SortOrder,
                IsDeleted = speciality.IsDeleted,
                No = speciality.No,
                Description = speciality.Description,
                Img = SPECIALITY_CONTENT_FOLDER_NAME + "/" + speciality.Img,
            };
            return new ApiSuccessResult<SpecialityVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(SpecialityUpdateRequest request)
        {
            var speciality = await _context.Specialities.FindAsync(request.Id);
            if (speciality == null) return new ApiSuccessResult<bool>(false);
            speciality.Title = request.Title;
            speciality.SortOrder = request.SortOrder;
            speciality.Description = request.Description;
            speciality.IsDeleted = request.IsDeleted;
            if (request.Img != null)
            {
                if (speciality.Img != null&& speciality.Img!="default") await _storageService.DeleteFileAsyncs(speciality.Img, SPECIALITY_CONTENT_FOLDER_NAME);
                speciality.Img = await SaveFile(request.Img, SPECIALITY_CONTENT_FOLDER_NAME);
            }

            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Cập nhật chuyên khoa không thành công!");
        }
    }
}
