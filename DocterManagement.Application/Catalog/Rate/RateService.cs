using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Rate
{
    public class RateService : IRateService
    {
        private readonly DoctorManageDbContext _context;

        public RateService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<Rates>> Create(RateCreateRequest request)
        {
            var rates = new Rates()
            {
                Title = request.Title,
                Date = DateTime.Now,
                Description = request.Description,
                Rating = request.Rating,
                AppointmentId = request.AppointmentId
            };
            _context.Rates.Add(rates);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Rates>(rates);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var rates = await _context.Rates.FindAsync(Id);
            int check = 0;
            if (rates == null) return new ApiSuccessResult<int>(check);
            _context.Rates.Remove(rates);
            check = 2;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<RateVm>>> GetAll()
        {
            var query = _context.Rates;

            var rs = await query.Select(x => new RateVm()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Rating = x.Rating,
                Date = x.Date,
                AppointmentId = x.AppointmentId
            }).ToListAsync();
            return new ApiSuccessResult<List<RateVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<RateVm>>> GetAllPaging(GetRatePagingRequest request)
        {
            var query = from c in _context.Rates select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Title.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new RateVm()
                {
                    Title = x.Title,
                    Description = x.Description,
                    Id = x.Id,
                    Rating = x.Rating,
                    AppointmentId = x.AppointmentId,
                    Date = x.Date

                }).ToListAsync();

            var pagedResult = new PagedResult<RateVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<RateVm>>(pagedResult);
        }

        public async Task<ApiResult<RateVm>> GetById(Guid Id)
        {
            var rates = await _context.Rates.FindAsync(Id);
            if (rates == null) throw new DoctorManageException($"Cannot find a rates with id: { Id}");
            var rs = new RateVm()
            {
                Id = rates.Id,
                Title = rates.Title,
                Description = rates.Description,
                Date = rates.Date,
                AppointmentId = rates.AppointmentId,
                Rating = rates.Rating
            };

            return new ApiSuccessResult<RateVm>(rs);
        }

        public async Task<ApiResult<Rates>> Update(RateUpdateRequest request)
        {
            var rates = await _context.Rates.FindAsync(request.Id);
            if (rates == null) throw new DoctorManageException($"Cannot find a rates with id: { request.Id}");
            rates.Title = request.Title;
            rates.Description = request.Description;
            rates.Rating = request.Rating;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Rates>(rates);
        }
    }
}
