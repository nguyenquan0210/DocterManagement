using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.ScheduleDetailt;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.ScheduleDetailt
{
    public class ScheduleDetailtService : IScheduleDetailtService
    {
        private readonly DoctorManageDbContext _context;

        public ScheduleDetailtService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> Create(ScheduleDetailtCreateRequest request)
        {
            var schedulesDetails = new SchedulesSlots()
            {
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                IsDeleted = false,
                ScheduleId = request.ScheduleId
            };
            _context.schedulesSlots.Add(schedulesDetails);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiSuccessResult<bool>(false);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var schedulesDetails = await _context.schedulesSlots.FindAsync(Id);
            int check = 0;
            if (schedulesDetails == null) return new ApiSuccessResult<int>(check);
            if (schedulesDetails.IsDeleted == true)
            {
                schedulesDetails.IsDeleted = false;
                check = 2;
            }
            /*else
            {
                _context.schedulesSlots.Remove(schedulesDetails);
                check = 2;
            }*/
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<ScheduleDetailtVm>>> GetAll()
        {
            var query = _context.schedulesSlots;

            var rs = await query.Select(x => new ScheduleDetailtVm()
            {
                Id = x.Id,
                FromTime = x.FromTime,
                ToTime = x.ToTime,
                ScheduleId = x.ScheduleId,
                IsDeleted = x.IsDeleted,
                IsBooked = x.IsBooked,
            }).ToListAsync();
            return new ApiSuccessResult<List<ScheduleDetailtVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<ScheduleDetailtVm>>> GetAllPaging(GetScheduleDetailtPagingRequest request)
        {
            var query = from c in _context.schedulesSlots select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.FromTime.ToString().Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ScheduleDetailtVm()
                {
                    Id = x.Id,
                    FromTime = x.FromTime,
                    ToTime = x.ToTime,
                    IsBooked = x.IsBooked,
                    IsDeleted=x.IsDeleted,
                    ScheduleId = x.ScheduleId

                }).ToListAsync();

            var pagedResult = new PagedResult<ScheduleDetailtVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<ScheduleDetailtVm>>(pagedResult);
        }

        public async Task<ApiResult<ScheduleDetailtVm>> GetById(Guid Id)
        {
            var schedules = await _context.schedulesSlots.FindAsync(Id);
            if (schedules == null) throw new DoctorManageException($"Cannot find a Schedule with id: { Id}");
            var rs = new ScheduleDetailtVm()
            {
                Id = schedules.Id,
                FromTime = schedules.FromTime,
                ToTime = schedules.ToTime,
                ScheduleId = schedules.ScheduleId,
                IsDeleted = schedules.IsDeleted,
                IsBooked= schedules.IsBooked,
            };
            return new ApiSuccessResult<ScheduleDetailtVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(ScheduleDetailtUpdateRequest request)
        {
            var schedulesDetails = await _context.schedulesSlots.FindAsync(request.Id);
            if (schedulesDetails == null) return new ApiSuccessResult<bool>(false);
            schedulesDetails.FromTime = request.FromTime;
            schedulesDetails.ToTime = request.ToTime;
            schedulesDetails.IsDeleted = request.IsDeleted;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiSuccessResult<bool>(false);
        }
    }
}
