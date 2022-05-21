using AutoMapper;
using DoctorManagement.Application.System.Users;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Models;
using DoctorManagement.ViewModels.System.Patient;
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
        private readonly IEmailService _emailService;

        public AppointmentService(DoctorManageDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<ApiResult<Guid>> Create(AppointmentCreateRequest request)
        {
            var slots = from slot in _context.schedulesSlots
                             join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                             where slot.Id == request.SchedulesSlotId 
                             select sche;
            var slotsche = await _context.schedulesSlots.FindAsync(request.SchedulesSlotId);
            var datecheck = new DateTime();
            var fromtime = new TimeSpan();
            var totime = new TimeSpan();
            var stt = 0;
            var scheduleId = slots.FirstOrDefault().Id;
            if (slots != null)
            {
                datecheck = DateTime.Parse(slots.FirstOrDefault().CheckInDate.ToString("dd/MM/yyyy") + " 00:00:00.0000000");
                fromtime = slots.FirstOrDefault().FromTime;
                totime = slots.FirstOrDefault().ToTime;
                var checkAppoi = from app in _context.Appointments
                                 join slot in _context.schedulesSlots on app.SchedulesSlotId equals slot.Id
                                 join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                                 where app.PatientId == request.PatientId && sche.CheckInDate >= datecheck && sche.CheckInDate < datecheck.AddDays(1) && sche.FromTime >= fromtime && sche.ToTime <= totime
                                 select sche;
                if (checkAppoi.ToList().Count > 0)
                {
                    return new ApiErrorResult<Guid>("Bạn không thể đặt lịch cùng ngày trên 1 khung giờ quá 2 lần!");
                }
                var slotcount = from slot in _context.schedulesSlots where slot.ScheduleId == scheduleId select slot;
                var i = 1;
                foreach(var x in slotcount)
                {
                    if(x.Id == slotsche.Id)
                    {
                        stt = i;
                    }
                    i++;
                }
            }
            string day = slots.FirstOrDefault().CheckInDate.ToString("dd") + "-" + request.DoctorId.ToString().Remove(2);
            int count = await _context.Doctors.Where(x => x.No.Contains("DK-" + day)).CountAsync();
            string str = "";
            if (count < 9) str = "DK-" + day + "-00" + (count + 1);
            else if (count < 99) str = "DK-" + day + "-0" + (count + 1);
            else if (count < 999) str = "DK-" + day + "-" + (count + 1);
            var appointments = new Appointments()
            {
                CreatedAt = DateTime.Now,
                Status = StatusAppointment.approved,
                PatientId = request.PatientId,
                No = str,
                SchedulesSlotId = request.SchedulesSlotId,
                DoctorId = request.DoctorId,
                Note = request.Note,
                IsDoctor = request.IsDoctor,
                Stt = stt + 1
            };
            if (request.formFiles != null)
            {
                appointments.Attachedfiles = new List<Attachedfiles>();
                foreach (var file in request.formFiles)
                {
                    var att = new Attachedfiles()
                    {
                        Img = file.Name,
                    };
                    appointments.Attachedfiles.Add(att);
                }
            }
           
            _context.Appointments.Add(appointments);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                
                slotsche.IsBooked = true;
                var schedule = await _context.Schedules.FindAsync(slots.FirstOrDefault().Id);
                schedule.AvailableQty = schedule.AvailableQty -1;
                schedule.BookedQty = schedule.BookedQty +1;
                _context.SaveChanges();
                var user = await _context.Users.FindAsync(request.DoctorId);
                var doctor = await _context.Doctors.FindAsync(request.DoctorId);
                var clinic = await _context.Clinics.FindAsync(doctor.ClinicId);
                var subdistrict = await _context.Locations.FindAsync(clinic.LocationId);
                var district = await _context.Locations.FindAsync(subdistrict.ParentId);
                var province = await _context.Locations.FindAsync(district.ParentId);
                clinic.Address = clinic.Address +", "+ subdistrict.Name + "," + district.Name + "," + province.Name;
                var patient = await _context.Patients.FindAsync(request.PatientId);
                var userpatient = await _context.Users.FindAsync(patient.UserId);
                var specialities = from ses in _context.ServicesSpecialities
                                   join s in _context.Specialities on ses.SpecialityId equals s.Id
                                   where ses.DoctorId == request.DoctorId && ses.IsDelete == false
                                   select new {s,ses};
                var i = 1;
                var ser = "";
                var cuot = specialities.Count();
                foreach (var speciality in specialities)
                {
                    ser = ser + speciality.s.Title + (i == cuot ? "" : "-").ToString();
                    i++;
                }
                doctor.Services = ser;
               
                if (user != null&& patient!= null) await SendEmailAppoitment(user, userpatient, patient, schedule, slotsche, appointments);
               
                return new ApiSuccessResult<Guid>(appointments.Id);
            }
            return new ApiErrorResult<Guid>("Đặt khám không thành công!");
        }
        private async Task SendEmailAppoitment(AppUsers user, AppUsers userpatient, Patients patients, Schedules schedules, SchedulesSlots schedulesSlots, Appointments appointment)
        {
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email , userpatient.Email},
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.UserName),
                    new KeyValuePair<string, string>("{{Stt}}", appointment.Stt.ToString()),
                    new KeyValuePair<string, string>("{{Address}}", user.Doctors.Clinics.Address),
                    new KeyValuePair<string, string>("{{Speciality}}", user.Doctors.Services),
                    new KeyValuePair<string, string>("{{DoctorName}}",user.Doctors.Prefix +" " + user.Doctors.LastName + " " + user.Doctors.FirstName),
                    new KeyValuePair<string, string>("{{PatientName}}", patients.Name),
                    new KeyValuePair<string, string>("{{PhoneNumber}}", userpatient.PhoneNumber),
                    new KeyValuePair<string, string>("{{DoctorPhone}}", user.PhoneNumber),
                    new KeyValuePair<string, string>("{{Date}}", schedules.CheckInDate.ToShortDateString()),
                    new KeyValuePair<string, string>("{{Note}}", appointment.Note),
                    new KeyValuePair<string, string>("{{TimeSpan}}", schedulesSlots.FromTime.ToString().Substring(0,5)+"-"+schedulesSlots.ToTime.ToString().Substring(0,5)),
                }
            };
            await _emailService.SendEmailAppoitment(options);
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
                CreatedAt = x.CreatedAt,
              /*  SchedulesDetailId = x.SchedulesSlotId,
                PatientId = x.PatientId,*/
                Status = x.Status
            }).ToListAsync();
            return new ApiSuccessResult<List<AppointmentVm>> (rs);
        }

        public async Task<ApiResult<PagedResult<AppointmentVm>>> GetAllPaging(GetAppointmentPagingRequest request)
        {
            var query = from a in _context.Appointments
                        join d in _context.Doctors on a.DoctorId equals d.UserId
                        join p in _context.Patients on a.PatientId equals p.PatientId
                        join slot in _context.schedulesSlots on a.SchedulesSlotId equals slot.Id
                        join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                        select new {a,d,p,slot,sche};
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.a.No.Contains(request.Keyword)|| x.p.Name.Contains(request.Keyword) ||
                x.d.FirstName.Contains(request.Keyword) || x.d.LastName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AppointmentVm()
                {
                    Id = x.a.Id,
                    CreatedAt = x.a.CreatedAt,
                    Patient = new PatientVm()
                    {
                        Id = x.p.PatientId,
                        Name = x.p.Name,
                        No = x.p.No,
                        Address = x.p.Address,
                    },
                    Schedule = new ScheduleVm()
                    {
                        Id = x.sche.Id,
                        CheckInDate = x.sche.CheckInDate,
                        FromTime = x.sche.FromTime,
                        ToTime = x.sche.ToTime,
                    },
                    SlotSchedule = new SlotScheduleVm()
                    {
                        Id = x.slot.Id,
                        FromTime = x.slot.FromTime,
                        ToTime = x.slot.ToTime,
                    },
                    Doctor = new DoctorVm()
                    {
                        UserId = x.d.UserId,
                        FirstName = x.d.FirstName,
                        LastName = x.d.LastName,
                        Address = x.d.Address,
                    },
                    Status = x.a.Status,
                    No = x.a.No,

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
            var appointments = await _context.Appointments.FindAsync(Id);
            if (appointments == null) return new ApiErrorResult<AppointmentVm>("phiếu đăng ký lịch khám không tồn tại!");
            var doctor = await _context.Doctors.FindAsync(appointments.DoctorId);
            var patient = await _context.Patients.FindAsync(appointments.PatientId);
            var slotsche = await _context.schedulesSlots.FindAsync(appointments.SchedulesSlotId);
            var schedule = await _context.Schedules.FindAsync(slotsche.ScheduleId);
            string[] locationIds = new string[] { patient.LocationId.ToString(), doctor.LocationId.ToString() };
            string fulladdress = "";
            for (var i = 0; i < locationIds.Length;i++)
            {
                var subdistrict = await _context.Locations.FindAsync(Guid.Parse(locationIds[i]));
                var district = await _context.Locations.FindAsync(subdistrict.ParentId);
                var province = await _context.Locations.FindAsync(district.ParentId);
                fulladdress = fulladdress +  subdistrict.Name + "," + district.Name + "," + province.Name + (i==0?";":"");
            }
            var rs = new AppointmentVm()
            {
                Id = appointments.Id,
                Status = appointments.Status,
                CreatedAt = appointments.CreatedAt,
                Stt = appointments.Stt,
                IsDoctor = appointments.IsDoctor,
                No = appointments.No,
                Note = appointments.Note,
                Schedule = new ScheduleVm()
                {
                    Id = schedule.Id,
                    CheckInDate = schedule.CheckInDate,
                    FromTime = schedule.FromTime,
                    ToTime = schedule.ToTime,
                },
                SlotSchedule = new SlotScheduleVm()
                {
                    Id = slotsche.Id,
                    FromTime = slotsche.FromTime,
                    ToTime = slotsche.ToTime,
                },
                Doctor = new DoctorVm()
                {
                    UserId = doctor.UserId,
                    No = doctor.No,
                    Prefix = doctor.Prefix,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    MapUrl = doctor.MapUrl,
                    FullAddress = doctor.Address+", " + fulladdress.Split(";")[1],
                    Img= doctor.Img
                },
                Patient = new PatientVm()
                {
                    Id = patient.PatientId,
                    Address = patient.Address,
                    FullAddress = patient.Address + ", " + fulladdress.Split(";")[0],
                    Name = patient.Name,
                    Img = patient.Img,
                    Dob = patient.Dob,
                    Gender = patient.Gender,
                    Identitycard = patient.Identitycard,
                    RelativePhone = patient.RelativePhone,
                    No = patient.No,
                }
            };
            return new ApiSuccessResult<AppointmentVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(AppointmentUpdateRequest request)
        {
            var appointments = await _context.Appointments.FindAsync(request.Id);
            if (appointments == null) throw new DoctorManageException($"Cannot find a Appointment with id: { request.Id}");
            
            appointments.Status = request.Status;
            await _context.SaveChangesAsync();
            return  new ApiSuccessResult<bool>();
        }
    }
}
