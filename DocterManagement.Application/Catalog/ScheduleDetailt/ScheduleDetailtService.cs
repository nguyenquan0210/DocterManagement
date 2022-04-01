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
        public async Task<Guid> Create(ScheduleDetailtCreateRequest request)
        {
            var schedulesDetails = new SchedulesDetails()
            {
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                Status = Data.Enums.Status.Active,
                ScheduleId = request.ScheduleId
            };
            _context.SchedulesDetails.Add(schedulesDetails);
            await _context.SaveChangesAsync();
            return schedulesDetails.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var schedulesDetails = await _context.SchedulesDetails.FindAsync(Id);
            int check = 0;
            if (schedulesDetails == null) return check;
            if (schedulesDetails.Status == Status.Active)
            {
                schedulesDetails.Status = Status.InActive;
                check = 1;
            }
            else
            {
                _context.SchedulesDetails.Remove(schedulesDetails);
                check = 2;
            }
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<ScheduleDetailtVm>> GetAll()
        {
            var query = _context.SchedulesDetails;

            return await query.Select(x => new ScheduleDetailtVm()
            {
                Id = x.Id,
                FromTime = x.FromTime,
                ToTime = x.ToTime,
                ScheduleId = x.ScheduleId,
                Status = x.Status
            }).ToListAsync();
        }

        public async Task<PagedResult<ScheduleDetailtVm>> GetAllPaging(GetScheduleDetailtPagingRequest request)
        {
            var query = from c in _context.SchedulesDetails select c;
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
                    Status = x.Status,
                    ScheduleId = x.ScheduleId

                }).ToListAsync();

            var pagedResult = new PagedResult<ScheduleDetailtVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<ScheduleDetailtVm> GetById(Guid Id)
        {
            var schedules = await _context.SchedulesDetails.FindAsync(Id);
            if (schedules == null) throw new DoctorManageException($"Cannot find a Schedule with id: { Id}");
            var rs = new ScheduleDetailtVm()
            {
                Id = schedules.Id,
                FromTime = schedules.FromTime,
                ToTime = schedules.ToTime,
                ScheduleId = schedules.ScheduleId,
                Status = schedules.Status
            };
            return rs;
        }

        public async Task<int> Update(ScheduleDetailtUpdateRequest request)
        {
            var schedules = await _context.SchedulesDetails.FindAsync(request.Id);
            if (schedules == null) throw new DoctorManageException($"Cannot find a Schedule with id: { request.Id}");
            schedules.FromTime = request.FromTime;
            schedules.ToTime = request.ToTime;
            schedules.Status = request.Status;
            return await _context.SaveChangesAsync();
        }
    }
}
