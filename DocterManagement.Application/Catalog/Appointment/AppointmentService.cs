using AutoMapper;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Appointment
{
    public class AppointmentService : IAppointmentService
    {
        private readonly DoctorManageDbContext _context;

        public AppointmentService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> Create(AppointmentCreateRequest request)
        {
            var slots = from slot in _context.schedulesSlots
                             join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                             where slot.Id == request.SchedulesSlotId 
                             select sche;
            var datecheck = new DateTime();
            if (slots != null)
            {
                datecheck = DateTime.Parse(slots.FirstOrDefault().CheckInDate.ToString("dd/MM/yyyy") + " 00:00:00.0000000");
                var checkAppoi = from app in _context.Appointments
                                 join slot in _context.schedulesSlots on app.SchedulesSlotId equals slot.Id
                                 join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                                 where app.PatientId == request.PatientId && sche.CheckInDate >= datecheck && sche.CheckInDate < datecheck.AddDays(1)
                                 select sche;
                if (checkAppoi.ToList().Count > 0)
                {
                    return new ApiErrorResult<bool>("Bạn không thể đătj lịch cùng ngày quá 2 lần!");
                }
            }

            string day = DateTime.Now.ToString("dd") + "-" + slots.FirstOrDefault().Id.ToString().Remove(2);
            int count = await _context.Doctors.Where(x => x.No.Contains("DK-" + day)).CountAsync();
            string str = "";
            if (count < 9) str = "DK-" + day + "-00" + (count + 1);
            else if (count < 99) str = "DK-" + day + "-0" + (count + 1);
            else if (count < 999) str = "DK-" + day + "-" + (count + 1);
            var appointments = new Appointments()
            {
                CreatedAt = DateTime.Now,
                Status = StatusAppointment.pending,
                PatientId = request.PatientId,
                No = str,
                SchedulesSlotId = request.SchedulesSlotId,
                DoctorId = request.DoctorId
            };
            _context.Appointments.Add(appointments);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var slotsche = await _context.schedulesSlots.FindAsync(request.SchedulesSlotId);
                slotsche.IsBooked = true;
                var schedule = await _context.Schedules.FindAsync(slots.FirstOrDefault().Id);
                schedule.AvailableQty = schedule.AvailableQty -1;
                schedule.BookedQty = schedule.BookedQty +1;
                _context.SaveChanges();
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Đặt khám không thành công!");
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var rs = await _context.Appointments.FindAsync(Id);
            int check = 0;
            if (rs == null) return new ApiSuccessResult<int>(check);
            if (rs.Status == StatusAppointment.pending)
            {
                rs.Status = StatusAppointment.approved;
                check = 1;
            }
            else
            {
                rs.Status = StatusAppointment.complete;
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<AppointmentVm>>> GetAll()
        {
            var query = _context.Appointments;

            var rs = await query.Select(x => new AppointmentVm()
            {
                Id = x.Id,
                Date = x.CreatedAt,
                SchedulesDetailId = x.SchedulesSlotId,
                PatientId = x.PatientId,
                Status = x.Status
            }).ToListAsync();
            return new ApiSuccessResult<List<AppointmentVm>> (rs);
        }

        public async Task<ApiResult<PagedResult<AppointmentVm>>> GetAllPaging(GetAppointmentPagingRequest request)
        {
            var query = from c in _context.Appointments select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.CreatedAt.ToShortDateString().Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AppointmentVm()
                {
                    Id = x.Id,
                    Date = x.CreatedAt,
                    PatientId = x.PatientId,
                    SchedulesDetailId = x.SchedulesSlotId,
                    Status = x.Status,
                    No = x.No,

                }).ToListAsync();

            var pagedResult = new PagedResult<AppointmentVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<AppointmentVm>> (pagedResult);
        }

        public async Task<ApiResult<AppointmentVm>> GetById(Guid Id)
        {
            var Appointments = await _context.Appointments.FindAsync(Id);
            if (Appointments == null) throw new DoctorManageException($"Cannot find a Appointment with id: { Id}");
            var rs = new AppointmentVm()
            {
                Id = Appointments.Id,
                Date = Appointments.CreatedAt,
                PatientId = Appointments.PatientId,
                SchedulesDetailId = Appointments.SchedulesSlotId,
                Status = Appointments.Status
            };
            return new ApiSuccessResult<AppointmentVm>(rs);
        }

        public async Task<ApiResult<Appointments>> Update(AppointmentUpdateRequest request)
        {
            var appointments = await _context.Appointments.FindAsync(request.Id);
            if (appointments == null) throw new DoctorManageException($"Cannot find a Appointment with id: { request.Id}");
            
            appointments.Status = request.Status;
            await _context.SaveChangesAsync();
            return  new ApiSuccessResult<Appointments>(appointments);
        }
    }
}
