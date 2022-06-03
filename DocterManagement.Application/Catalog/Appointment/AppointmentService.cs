﻿using AutoMapper;
using DoctorManagement.Application.System.Users;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Models;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
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
            string day = slots.FirstOrDefault().CheckInDate.ToString("dd") + request.DoctorId.ToString().Substring(0,1) + request.DoctorId.ToString().Substring(17, 18) + request.DoctorId.ToString().Substring(32, 33);
            int count = await _context.Doctors.Where(x => x.No.Contains("DMDK" + day)).CountAsync();
            string str = "";
            if (count < 9) str = "DMDK" + day + "0000" + (count + 1);
            else if (count < 99) str = "DMDK" + day + "000" + (count + 1);
            else if (count < 999) str = "DMDK" + day + "00" + (count + 1);
            else if (count < 9999) str = "DMDK" + day + "0" + (count + 1);
            else if (count < 99999) str = "DMDK" + day  + (count + 1);
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
                Stt = stt + 1,
                CancelDate = new DateTime()
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
                
               
                if (user != null&& patient!= null) await SendEmailAppoitment(user, userpatient, patient, schedule, slotsche, appointments, ser);
               
                return new ApiSuccessResult<Guid>(appointments.Id);
            }
            return new ApiErrorResult<Guid>("Đặt khám không thành công!");
        }
        private async Task SendEmailAppoitment(AppUsers user, AppUsers userpatient, Patients patients, Schedules schedules, SchedulesSlots schedulesSlots, Appointments appointment, string str)
        {
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email , userpatient.Email},
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.UserName),
                    new KeyValuePair<string, string>("{{Stt}}", appointment.Stt.ToString()),
                    new KeyValuePair<string, string>("{{Address}}", user.Doctors.Clinics.Address),
                    new KeyValuePair<string, string>("{{Speciality}}", str),
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
                        join ud in _context.AppUsers on d.UserId equals ud.Id
                        join p in _context.Patients on a.PatientId equals p.PatientId
                        join u in _context.AppUsers on p.UserId equals u.Id
                        join slot in _context.schedulesSlots on a.SchedulesSlotId equals slot.Id
                        join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                        join m in _context.MedicalRecords on a.Id equals m.AppointmentId into me
                        from m in me.DefaultIfEmpty()
                        select new {a,d,p,slot,sche,u,m,ud};

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.a.No.Contains(request.Keyword)|| x.p.Name.Contains(request.Keyword) || x.p.RelativePhone.Contains(request.Keyword) ||
                x.d.FirstName.Contains(request.Keyword) || x.d.LastName.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.u.UserName == request.UserName);
            }
            if (!string.IsNullOrEmpty(request.UserNameDoctor))
            {
                query = query.Where(x => x.ud.UserName == request.UserNameDoctor);
            }
            if (request.status != null)
            {
                query = query.Where(x => x.a.Status == request.status);
            }
            int totalRow = await query.CountAsync();
            query = query.OrderByDescending(x=>x.a.Status).ThenBy(x => x.sche.CheckInDate);
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AppointmentVm()
                {
                    Id = x.a.Id,
                    CreatedAt = x.a.CreatedAt,
                    Status = x.a.Status,
                    No = x.a.No,
                    Stt = x.a.Stt,
                    IsDoctor = x.a.IsDoctor,
                    Note = x.a.Note,
                    Patient = new PatientVm()
                    {
                        Id = x.p.PatientId,
                        Name = x.p.Name,
                        No = x.p.No,
                        Address = x.p.Address,
                        Dob = x.p.Dob,
                        FullAddress = x.p.FullAddress,
                        Gender = x.p.Gender,
                        Identitycard = x.p.Identitycard,
                        RelativeName = x.p.RelativeName,
                        RelativePhone = x.p.RelativePhone,
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
                        Prefix = x.d.Prefix,
                        FullName = x.d.Prefix+" "+ x.d.LastName + " " + x.d.FirstName,
                        FullAddress = x.d.FullAddress,
                        Img = "user-content/"+ x.d.Img,
                    },
                    MedicalRecord = x.m !=null ? new MedicalRecordVm()
                    {
                        Id = x.m.Id,
                        Status = x.m.Status,
                        StatusIllness = x.m.StatusIllness,
                        CreateAt = x.m.CreatedAt,
                        Diagnose = x.m.Diagnose,
                        Note = x.m.Note,
                    }: new MedicalRecordVm()

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
            var userdoctor = await _context.AppUsers.FindAsync(appointments.DoctorId);
            var patient = await _context.Patients.FindAsync(appointments.PatientId);
            var slotsche = await _context.schedulesSlots.FindAsync(appointments.SchedulesSlotId);
            var schedule = await _context.Schedules.FindAsync(slotsche.ScheduleId);
            var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(x=>x.AppointmentId==appointments.Id);
            var medicine = from md in _context.MedicineDetailts join m in _context.Medicines on md.MedicineId equals m.Id where md.MedicalRecordId == medicalRecord.Id select new {m,md};
            var services = from sd in _context.ServiceDetailts join s in _context.Services on sd.ServicesId equals s.Id where sd.MedicalRecordId == medicalRecord.Id select new { s, sd };
            var rs = new AppointmentVm()
            {
                Id = appointments.Id,
                Status = appointments.Status,
                CreatedAt = appointments.CreatedAt,
                Stt = appointments.Stt,
                IsDoctor = appointments.IsDoctor,
                No = appointments.No,
                Note = appointments.Note,
                CancelReason = appointments.CancelReason,
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
                    FullAddress = doctor.FullAddress,
                    Img = "user-content/" + doctor.Img,
                    FullName = doctor.Prefix +" "+ doctor.LastName + " " + doctor.FirstName,
                    User = new UserVm()
                    {
                        Email = userdoctor.Email,
                        PhoneNumber = userdoctor.PhoneNumber,
                    }
                },
                Patient = new PatientVm()
                {
                    Id = patient.PatientId,
                    Address = patient.Address,
                    FullAddress = patient.FullAddress,
                    Name = patient.Name,
                    Img = patient.Img,
                    Dob = patient.Dob,
                    Gender = patient.Gender,
                    Identitycard = patient.Identitycard,
                    RelativePhone = patient.RelativePhone,
                    RelativeEmail = patient.RelativeEmail,
                    No = patient.No,
                },
                MedicalRecord = medicalRecord == null? new MedicalRecordVm() : new MedicalRecordVm()
                {
                    Id = medicalRecord.Id,
                    Status = medicalRecord.Status,
                    StatusIllness = medicalRecord.StatusIllness,
                    CreateAt = medicalRecord.CreatedAt,
                    AppointmentId = medicalRecord.AppointmentId,
                    Diagnose = medicalRecord.Diagnose,
                    Note = medicalRecord.Note,
                    TotalAmount = medicalRecord.TotalAmount,
                    Service = services.Select(x=> new ServiceCreate()
                    {
                        ServiceId = x.sd.ServicesId,
                        TotalAmountString = x.sd.TotalAmount.ToString("#,###,### vnđ"),
                        Name = x.s.ServiceName,
                        Price = x.sd.Price,
                        Qty = x.sd.Qty,
                        Unit = x.s.Unit,
                        TotalAmount = x.sd.TotalAmount
                    }).ToList(),
                    Medicine = medicine==null?null: medicine.Select(x => new MedicineCreate()
                    {
                        MedicineId = x.md.MedicineId,
                        TotalAmountString = x.md.TotalAmount.ToString("#,###,### vnđ"),
                        Name = x.m.Name,
                        Price = x.md.Price,
                        Qty = x.md.Qty,
                        Unit = x.m.Unit,
                        TotalAmount = x.md.TotalAmount,
                        Afternoon = x.md.Afternoon,
                        Morning = x.md.Morning,
                        Night = x.md.Night,
                        Noon = x.md.Noon,
                        Use = x.md.Use
                    }).ToList()
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

        public async Task<ApiResult<bool>> CanceledAppointment(AppointmentCancelRequest request)
        {
            var appointments = await _context.Appointments.FindAsync(request.Id);
            if (appointments == null) return new ApiErrorResult<bool>("null");
            appointments.Status = StatusAppointment.cancel;
            appointments.CancelReason = request.CancelReason;
            appointments.CancelDate = DateTime.Now;
            var slot = await _context.schedulesSlots.FindAsync(appointments.SchedulesSlotId);
            var schedule = await _context.Schedules.FindAsync(slot.ScheduleId);
            if (request.Checked== "patient"){
                
                slot.IsBooked = false;
                
                schedule.BookedQty = schedule.BookedQty - 1;
                schedule.AvailableQty = schedule.AvailableQty + 1;
            }
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var user = await _context.Users.FindAsync(appointments.DoctorId);
                var doctor = await _context.Doctors.FindAsync(appointments.DoctorId);
                var patient = await _context.Patients.FindAsync(appointments.PatientId);
                var userpatient = await _context.Users.FindAsync(patient.UserId);
                if (user != null && patient != null) await SendEmailCancelAppoitment(user, userpatient, patient, schedule, slot, appointments);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("");
        }
        private async Task SendEmailCancelAppoitment(AppUsers user, AppUsers userpatient, Patients patients, Schedules schedules, SchedulesSlots schedulesSlots, Appointments appointment)
        {
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email, userpatient.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{DoctorName}}",user.Doctors.Prefix +" " + user.Doctors.LastName + " " + user.Doctors.FirstName),
                    new KeyValuePair<string, string>("{{PatientName}}", patients.Name),
                    new KeyValuePair<string, string>("{{Date}}", schedules.CheckInDate.ToShortDateString()),
                    new KeyValuePair<string, string>("{{CancelReason}}", appointment.CancelReason),
                    new KeyValuePair<string, string>("{{No}}", appointment.No),
                    new KeyValuePair<string, string>("{{CancelDate}}", appointment.CancelDate.ToString("HH:mm:ss dd/MM/yyyy")),
                    new KeyValuePair<string, string>("{{TimeSpan}}", schedulesSlots.FromTime.ToString().Substring(0,5)+"-"+schedulesSlots.ToTime.ToString().Substring(0,5)),
                }
            };
            await _emailService.SendEmailCancelAppoitment(options);
        }

        public async Task<ApiResult<bool>> AddExpired(GetAppointmentPagingRequest request)
        {
            var expired = from a in _context.Appointments
                          join d in _context.Doctors on a.DoctorId equals d.UserId
                          join ud in _context.AppUsers on d.UserId equals ud.Id
                          join p in _context.Patients on a.PatientId equals p.PatientId
                          join u in _context.AppUsers on p.UserId equals u.Id
                          join slot in _context.schedulesSlots on a.SchedulesSlotId equals slot.Id
                          join sche in _context.Schedules on slot.ScheduleId equals sche.Id
                          where sche.CreatedAt <= DateTime.Now.AddHours(-12) && a.Status == StatusAppointment.approved
                          select new { a, u, ud};
            if (!string.IsNullOrEmpty(request.UserName))
            {
                expired = expired.Where(x => x.u.UserName == request.UserName);
            }
            if (!string.IsNullOrEmpty(request.UserNameDoctor))
            {
                expired = expired.Where(x => x.ud.UserName == request.UserNameDoctor);
            }
            foreach (var remove in expired)
            {
                var addexpired = await _context.Appointments.FindAsync(remove.a.Id);
                addexpired.Status = StatusAppointment.pending;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }
    }
}
