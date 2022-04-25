using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Schedule
{
    public class ScheduleService : IScheduleService
    {
        private readonly DoctorManageDbContext _context;

        public ScheduleService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> Create(ScheduleCreateRequest request)
        {
            var schedules = new Schedules()
            {
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                CheckInDate = request.CheckInDate,
                Status = Data.Enums.Status.Active,
                Qty = request.Qty,
                DoctorId = request.DoctorId
            };
            _context.Schedules.Add(schedules);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiSuccessResult<bool>(false);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var schedules = await _context.Schedules.FindAsync(Id);
            int check = 0;
            if (schedules == null) return new ApiSuccessResult<int>(check);
            if (schedules.Status == Status.Active)
            {
                schedules.Status = Status.InActive;
                check = 1;
            }
            else
            {
                _context.Schedules.Remove(schedules);
                check = 2;
            }
            await _context.SaveChangesAsync();
             return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<ScheduleVm>>> GetAll()
        {
            var query = _context.Schedules;

            var rs = await query.Select(x => new ScheduleVm()
            {
                Id = x.Id,
                CheckInDate = x.CheckInDate,
                FromTime = x.FromTime,
                ToTime = x.ToTime,
                DoctorId = x.DoctorId,
                Qty = x.Qty,
                Status = x.Status
            }).ToListAsync();
            return new ApiSuccessResult<List<ScheduleVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<ScheduleVm>>> GetAllPaging(GetSchedulePagingRequest request)
        {
            var query = from c in _context.Schedules select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.CheckInDate.ToShortDateString().Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ScheduleVm()
                {
                    Id = x.Id,
                    CheckInDate = x.CheckInDate,
                    FromTime = x.FromTime,
                    ToTime = x.ToTime,
                    Qty = x.Qty,
                    Status = x.Status,
                    DoctorId = x.DoctorId

                }).ToListAsync();

            var pagedResult = new PagedResult<ScheduleVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<ScheduleVm>>(pagedResult);
        }

        public async Task<ApiResult<ScheduleVm>> GetById(Guid Id)
        {
            var schedules = await _context.Schedules.FindAsync(Id);
            if (schedules == null) throw new DoctorManageException($"Cannot find a Schedule with id: { Id}");
            var rs = new ScheduleVm()
            {
                Id = schedules.Id,
                CheckInDate = schedules.CheckInDate,
                FromTime = schedules.FromTime,
                ToTime = schedules.ToTime,
                DoctorId = schedules.DoctorId,
                Qty = schedules.Qty,
                Status = schedules.Status
            };
            return new ApiSuccessResult<ScheduleVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(ScheduleUpdateRequest request)
        {
            var schedules = await _context.Schedules.FindAsync(request.Id);
            if (schedules == null) return new ApiSuccessResult<bool>(false);
            schedules.FromTime = request.FromTime;
            schedules.ToTime = request.ToTime;
            schedules.Qty = request.Qty;
            schedules.Status = request.Status;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiSuccessResult<bool>(false);
        }
    }
}
