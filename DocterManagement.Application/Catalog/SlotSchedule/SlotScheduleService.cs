using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.SlotSchedule
{
    public class SlotScheduleService : ISlotScheduleService
    {
        private readonly DoctorManageDbContext _context;

        public SlotScheduleService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> Create(SlotScheduleCreateRequest request)
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

        public async Task<ApiResult<List<SlotScheduleVm>>> GetAll()
        {
            var query = _context.schedulesSlots;

            var rs = await query.Select(x => new SlotScheduleVm()
            {
                Id = x.Id,
                FromTime = x.FromTime,
                ToTime = x.ToTime,
                ScheduleId = x.ScheduleId,
                IsDeleted = x.IsDeleted,
                IsBooked = x.IsBooked,
            }).ToListAsync();
            return new ApiSuccessResult<List<SlotScheduleVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<SlotScheduleVm>>> GetAllPaging(GetSlotSchedulePagingRequest request)
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
                .Select(x => new SlotScheduleVm()
                {
                    Id = x.Id,
                    FromTime = x.FromTime,
                    ToTime = x.ToTime,
                    IsBooked = x.IsBooked,
                    IsDeleted = x.IsDeleted,
                    ScheduleId = x.ScheduleId

                }).ToListAsync();

            var pagedResult = new PagedResult<SlotScheduleVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<SlotScheduleVm>>(pagedResult);
        }

        public async Task<ApiResult<SlotScheduleVm>> GetById(Guid Id)
        {
            var schedulesSlot = await _context.schedulesSlots.FindAsync(Id);
            if (schedulesSlot == null) return new ApiErrorResult<SlotScheduleVm>("lịch khám không tồn tại");
            var schedules = await _context.Schedules.FindAsync(schedulesSlot.ScheduleId);

            var rs = new SlotScheduleVm()
            {
                Id = schedulesSlot.Id,
                FromTime = schedulesSlot.FromTime,
                ToTime = schedulesSlot.ToTime,
                ScheduleId = schedulesSlot.ScheduleId,
                IsDeleted = schedulesSlot.IsDeleted,
                IsBooked = schedulesSlot.IsBooked,
                Schedule = new ScheduleVm()
                {
                    Id = schedules.Id,
                    CheckInDate = schedules.CheckInDate
                }
            };
            return new ApiSuccessResult<SlotScheduleVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(SlotScheduleUpdateRequest request)
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
