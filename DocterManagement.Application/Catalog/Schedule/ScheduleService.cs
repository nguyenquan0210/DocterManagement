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
            var user = await _context.AppUsers.FirstOrDefaultAsync(x=>x.UserName == request.Username);
            if(user == null) return new ApiErrorResult<bool>("Tài khoản này không được phép!");
            var manyday = request.ToDay - request.FromDay;
            var manytime = request.ToTime - request.FromTime;
            var minutes = (manytime.Minutes + (manytime.Hours * 60)) / request.Qty;
            var fromtime = request.FromTime.Add(TimeSpan.FromMinutes(minutes));
            var day = request.FromDay;
            var listschedule = await _context.Schedules.Where(x=>x.DoctorId == user.Id && x.CheckInDate >= request.FromDay && x.CheckInDate <= request.ToDay).ToListAsync();
            if (listschedule.Any())
            {
                foreach (var remove in listschedule)
                {
                    _context.Schedules.Remove(remove);
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
                    schedules.SchedulesDetails = new List<SchedulesDetailts>();
                    for (var j = 1; j <= request.Qty; j++)
                    {

                        var scheduledetailt = new SchedulesDetailts()
                        {
                            FromTime = fromtime,
                            ToTime = fromtime.Add(TimeSpan.FromMinutes(minutes)),
                            Status = Status.Active
                        };
                        schedules.SchedulesDetails.Add(scheduledetailt);
                        fromtime = fromtime.Add(TimeSpan.FromMinutes(minutes));
                    }
                    _context.Schedules.Add(schedules);
                }
                
                day = day.AddDays(i);
            }
           
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
