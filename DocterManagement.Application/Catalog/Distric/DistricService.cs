using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Distric;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Distric
{
    public class DistricService : IDistricService
    {
        private readonly DoctorManageDbContext _context;

        public DistricService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<Districs>> Create(DistricCreateRequest request)
        {
            var rs = new Districs()
            {
                Name = request.Name,
                SortOrder = request.SortOrder
            };
            _context.Districs.Add(rs);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Districs>(rs);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var districs = await _context.Districs.FindAsync(Id);
            int check = 0;
            if (districs == null) return new ApiSuccessResult<int>(check);

            _context.Districs.Remove(districs);
                check = 2;
            
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<DistricVm>>> GetAll()
        {
            var query = _context.Districs;

            var rs = await query.Select(x => new DistricVm()
            {
                Id = x.Id,
                Name = x.Name,
                SortOrder = x.SortOrder
            }).ToListAsync();
            return new ApiSuccessResult<List<DistricVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<DistricVm>>> GetAllPaging(GetDistricPagingRequest request)
        {
            var query = from c in _context.Districs select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DistricVm()
                {
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    Id = x.Id

                }).ToListAsync();

            var pagedResult = new PagedResult<DistricVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<DistricVm>>(pagedResult);
        }

        public async Task<ApiResult<DistricVm>> GetById(Guid Id)
        {
            var Distric = await _context.Districs.FindAsync(Id);
            if (Distric == null) throw new DoctorManageException($"Cannot find a distric with id: { Id}");
            var rs = new DistricVm()
            {
                Id = Distric.Id,
                Name = Distric.Name,
                SortOrder = Distric.SortOrder
            };

            return new ApiSuccessResult<DistricVm>(rs);
        }

        public async Task<ApiResult<Districs>> Update(DistricUpdateRequest request)
        {
            var districs = await _context.Districs.FindAsync(request.Id);
            if (districs == null) throw new DoctorManageException($"Cannot find a distric with id: { request.Id}");
            districs.Name = request.Name;
            districs.SortOrder = request.SortOrder;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Districs>(districs);
        }
    }
}
