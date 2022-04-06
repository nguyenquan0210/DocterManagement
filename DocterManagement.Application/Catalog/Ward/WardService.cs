using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Ward;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Ward
{
    public class WardService : IWardService
    {
        private readonly DoctorManageDbContext _context;

        public WardService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<Wards>> Create(WardCreateRequest request)
        {
            var wards = new Wards()
            {
                Name = request.Name,
                SortOrder = request.SortOrder,
                DisticId = request.DisticId
            };
            _context.Wards.Add(wards);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Wards>(wards);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var wards = await _context.Wards.FindAsync(Id);
            int check = 0;
            if (wards == null) return new ApiSuccessResult<int>(check);

            _context.Wards.Remove(wards);
            check = 2;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<WardVm>>> GetAll()
        {
            var query = _context.Wards;

            var rs = await query.Select(x => new WardVm()
            {
                Id = x.Id,
                Name = x.Name,
                SortOrder = x.SortOrder,
                DisticId = x.DisticId
            }).ToListAsync();
            return new ApiSuccessResult<List<WardVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<WardVm>>> GetAllPaging(GetWardPagingRequest request)
        {
            var query = from c in _context.Wards select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new WardVm()
                {
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    Id = x.Id,
                    DisticId = x.DisticId

                }).ToListAsync();

            var pagedResult = new PagedResult<WardVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<WardVm>>(pagedResult);
        }

        public async Task<ApiResult<WardVm>> GetById(Guid Id)
        {
            var wards = await _context.Wards.FindAsync(Id);
            if (wards == null) throw new DoctorManageException($"Cannot find a Ward with id: { Id}");
            var rs = new WardVm()
            {
                Id = wards.Id,
                Name = wards.Name,
                SortOrder = wards.SortOrder,
                DisticId = wards.DisticId
            };

            return new ApiSuccessResult<WardVm>(rs);
        }

        public async Task<ApiResult<Wards>> Update(WardUpdateRequest request)
        {
            var wards = await _context.Wards.FindAsync(request.Id);
            if (wards == null) throw new DoctorManageException($"Cannot find a Ward with id: { request.Id}");
            wards.Name = request.Name;
            wards.SortOrder = request.SortOrder;
            wards.DisticId = request.DisticId;

            return new ApiSuccessResult<Wards>(wards);
        }
    }
}
