using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Speciality
{
    public class SpecialityService : ISpecialityService
    {
        private readonly DoctorManageDbContext _context;

        public SpecialityService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<Specialities>> Create(SpecialityCreateRequest request)
        {
            var specialities = new Specialities()
            {
                Title = request.Title,
                SortOrder = request.SortOrder,
                Description = request.Description
            };
            _context.Specialities.Add(specialities);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Specialities>(specialities);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var speciality = await _context.Specialities.FindAsync(Id);
            int check = 0;
            if (speciality == null) return new ApiSuccessResult<int>(check);
            if (speciality.Status == Status.Active)
            {
                speciality.Status = Status.InActive;
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

        public async Task<ApiResult<List<SpecialityVm>>> GetAll()
        {
            var query = _context.Specialities.Where(x => x.Status == Status.Active);

            var rs = await query.Select(x => new SpecialityVm()
            {
                Id = x.Id,
                Title = x.Title,
                SortOrder = x.SortOrder,
                Status = x.Status
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
                    Status = x.Status

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
            if (speciality == null) throw new DoctorManageException($"Cannot find a speciality with id: { Id}");
            var rs = new SpecialityVm()
            {
                Id = speciality.Id,
                Title = speciality.Title,
                SortOrder = speciality.SortOrder,
                Status = speciality.Status
            };

            return new ApiSuccessResult<SpecialityVm>(rs);
        }

        public async Task<ApiResult<Specialities>> Update(SpecialityUpdateRequest request)
        {
            var speciality = await _context.Specialities.FindAsync(request.Id);
            if (speciality == null) throw new DoctorManageException($"Cannot find a speciality with id: { request.Id}");
            speciality.Title = request.Title;
            speciality.SortOrder = request.SortOrder;
            speciality.Description = request.Description;
            speciality.Status = request.Status ? Status.Active : Status.InActive;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Specialities>(speciality);
        }
    }
}
