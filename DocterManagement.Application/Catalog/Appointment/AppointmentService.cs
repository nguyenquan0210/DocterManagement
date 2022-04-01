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
        public async Task<Guid> Create(AppointmentCreateRequest request)
        {
            var appointments = new Appointments()
            {
                Date = DateTime.Now,
                Status = Data.Enums.StatusAppointment.pending,
                PatientId = request.PatientId,
                SchedulesDetailId = request.SchedulesDetailId
            };
            _context.Appointments.Add(appointments);
            await _context.SaveChangesAsync();
            return  appointments.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var Appointments = await _context.Appointments.FindAsync(Id);
            int check = 0;
            if (Appointments == null) return check;
            if (Appointments.Status == StatusAppointment.pending)
            {
                Appointments.Status = StatusAppointment.approved;
                check = 1;
            }
            else
            {
                Appointments.Status = StatusAppointment.complete;
                check = 2;
            }
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<AppointmentVm>> GetAll()
        {
            var query = _context.Appointments;

            return await query.Select(x => new AppointmentVm()
            {
                Id = x.Id,
                Date = x.Date,
                SchedulesDetailId = x.SchedulesDetailId,
                PatientId = x.PatientId,
                Status = x.Status
            }).ToListAsync();
        }

        public async Task<PagedResult<AppointmentVm>> GetAllPaging(GetAppointmentPagingRequest request)
        {
            var query = from c in _context.Appointments select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Date.ToShortDateString().Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AppointmentVm()
                {
                    Id = x.Id,
                    Date = x.Date,
                    PatientId = x.PatientId,
                    SchedulesDetailId = x.SchedulesDetailId,
                    Status = x.Status

                }).ToListAsync();

            var pagedResult = new PagedResult<AppointmentVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<AppointmentVm> GetById(Guid Id)
        {
            var Appointments = await _context.Appointments.FindAsync(Id);
            if (Appointments == null) throw new DoctorManageException($"Cannot find a Appointment with id: { Id}");
            var rs = new AppointmentVm()
            {
                Id = Appointments.Id,
                Date = Appointments.Date,
                PatientId = Appointments.PatientId,
                SchedulesDetailId = Appointments.SchedulesDetailId,
                Status = Appointments.Status
            };
            return rs;
        }

        public async Task<int> Update(AppointmentUpdateRequest request)
        {
            var Appointments = await _context.Appointments.FindAsync(request.Id);
            if (Appointments == null) throw new DoctorManageException($"Cannot find a Appointment with id: { request.Id}");
            
            Appointments.Status = request.Status;
            return await _context.SaveChangesAsync();
        }
    }
}
