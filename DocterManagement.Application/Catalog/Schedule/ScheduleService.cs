using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Catalog.ScheduleDetailt;
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
            var user = await _context.AppUsers.FirstOrDefaultAsync(x=>x.UserName == request.Username);
            if(user == null) return new ApiErrorResult<bool>("Tài khoản này không được phép!");
            var manyday = request.ToDay - request.FromDay;
            var manytime = request.ToTime - request.FromTime;
            var minutes = (manytime.Minutes + (manytime.Hours * 60)) / request.Qty;
            var fromtime = request.FromTime;
            var day = request.FromDay;
            var listschedule = await _context.Schedules.Where(x=>x.DoctorId == user.Id && x.CheckInDate >= request.FromDay && x.CheckInDate <= request.ToDay).ToListAsync();
            if (listschedule.Any())
            {
                foreach (var remove in listschedule)
                {
                    var removeschedule = await _context.Schedules.FindAsync(remove.Id);
                    if (removeschedule != null)
                        _context.Schedules.Remove(removeschedule);
                }
            }
            for(var i = 1; i <= manyday.Days; i++)
            {
                if(day.ToString("dddd") == request.WeekDay)
                {
                    var schedules = new Schedules()
                    {
                        FromTime = request.FromTime,
                        ToTime = request.ToTime,
                        CheckInDate = day,
                        Status = Status.Active,
                        Qty = request.Qty,
                        DoctorId = user.Id
                    };
                    schedules.schedulesSlots = new List<SchedulesSlots>();
                    for (var j = 1; j <= request.Qty; j++)
                    {
                        var scheduledetailt = new SchedulesSlots()
                        {
                            FromTime = fromtime,
                            ToTime = fromtime.Add(TimeSpan.FromMinutes(minutes)),
                            Status = Status.Active
                        };
                        schedules.schedulesSlots.Add(scheduledetailt);
                        fromtime = fromtime.Add(TimeSpan.FromMinutes(minutes));
                    }
                    _context.Schedules.Add(schedules);
                }
                day = day.AddDays(1);
            }
           
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Tạo lịch khám không thành công!!!");
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
                //_context.Schedules.Remove(schedules);
                schedules.Status = Status.NotActivate;
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
            var schedule = await _context.Schedules.FindAsync(Id);
            var scheduledetailts = _context.schedulesSlots.Where(x=>x.ScheduleId == schedule.Id);
            if (schedule == null) throw new DoctorManageException($"Cannot find a Schedule with id: { Id}");
            var rs = new ScheduleVm()
            {
                Id = schedule.Id,
                CheckInDate = schedule.CheckInDate,
                FromTime = schedule.FromTime,
                ToTime = schedule.ToTime,
                DoctorId = schedule.DoctorId,
                Qty = schedule.Qty,
                Status = schedule.Status,
                ScheduleDetailts = scheduledetailts.Select(x=> new ScheduleDetailtVm()
                {
                    Id = x.Id,
                    FromTime = x.FromTime,
                    ToTime = x.ToTime,
                    Status = x.Status,
                }).ToList()
            };
            return new ApiSuccessResult<ScheduleVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(ScheduleUpdateRequest request)
        {
            var schedules = await _context.Schedules.FindAsync(request.Id);
            if (schedules == null) return new ApiErrorResult<bool>("Lịch khám không tồn tại!!!");
            if (schedules.FromTime != request.FromTime || schedules.ToTime != request.ToTime
                || schedules.Qty != request.Qty)
            {
                var scheduledetailts = _context.schedulesSlots.Where(x => x.ScheduleId == schedules.Id);
                foreach (var remove in scheduledetailts)
                {
                    var removescheduledetailt = await _context.schedulesSlots.FindAsync(remove.Id);
                    if(removescheduledetailt != null)
                        _context.schedulesSlots.Remove(removescheduledetailt);
                    
                }
                var manytime = request.ToTime - request.FromTime;
                var minutes = (manytime.Minutes + (manytime.Hours * 60)) / request.Qty;
                var fromtime = request.FromTime;
                schedules.schedulesSlots = new List<SchedulesSlots>();
                for (var j = 1; j <= request.Qty; j++)
                {
                    var scheduledetailt = new SchedulesSlots()
                    {
                        FromTime = fromtime,
                        ToTime = fromtime.Add(TimeSpan.FromMinutes(minutes)),
                        Status = Status.Active
                    };
                    schedules.schedulesSlots.Add(scheduledetailt);
                    fromtime = fromtime.Add(TimeSpan.FromMinutes(minutes));
                }
            }
            schedules.FromTime = request.FromTime;
            schedules.ToTime = request.ToTime;
            schedules.Qty = request.Qty;
            schedules.Status = request.Status? Status.Active:Status.InActive;

            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Cập nhật không thành công!!!");
        }
    }
}
