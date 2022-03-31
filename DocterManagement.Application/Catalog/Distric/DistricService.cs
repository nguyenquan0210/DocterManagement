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
        public async Task<Guid> Create(DistricCreateRequest request)
        {
            var districs = new Districs()
            {
                Name = request.Name,
                SortOrder = request.SortOrder
            };
            _context.Districs.Add(districs);
            await _context.SaveChangesAsync();
            return districs.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var districs = await _context.Districs.FindAsync(Id);
            int check = 0;
            if (districs == null) return check;
            
                _context.Districs.Remove(districs);
                check = 2;
            
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<DistricVm>> GetAll()
        {
            var query = _context.Districs;

            return await query.Select(x => new DistricVm()
            {
                Id = x.Id,
                Name = x.Name,
                SortOrder = x.SortOrder
            }).ToListAsync();
        }

        public async Task<PagedResult<DistricVm>> GetAllPaging(GetDistricPagingRequest request)
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
            return pagedResult;
        }

        public async Task<DistricVm> GetById(Guid Id)
        {
            var Distric = await _context.Districs.FindAsync(Id);
            if (Distric == null) throw new DoctorManageException($"Cannot find a distric with id: { Id}");
            var rs = new DistricVm()
            {
                Id = Distric.Id,
                Name = Distric.Name,
                SortOrder = Distric.SortOrder
            };

            return rs;
        }

        public async Task<int> Update(DistricUpdateRequest request)
        {
            var Distric = await _context.Districs.FindAsync(request.Id);
            if (Distric == null) throw new DoctorManageException($"Cannot find a distric with id: { request.Id}");
            Distric.Name = request.Name;
            Distric.SortOrder = request.SortOrder;

            return await _context.SaveChangesAsync();
        }
    }
}
