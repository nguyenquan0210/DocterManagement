using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.MedicalRecords
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly DoctorManageDbContext _context;

        public MedicalRecordService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> Create(MedicalRecordCreateRequest request)
        {
            var medical = new MedicalRecord()
            {
                Diagnose = request.Diagnose,
                Note = request.Note == null?"Null" : request.Note,
                StatusIllness = request.StatusIllness,
                CreatedAt = DateTime.Now,
                Status = Status.Active,
                TotalAmount = (request.Service.Sum(x=>x.Price.Value)* request.Service.Sum(x => x.Qty)) + (request.Medicine != null? (request.Medicine.Sum(x => x.Price.Value)* request.Medicine.Sum(x => x.Qty)) :0),
                AppointmentId = request.AppointmentId,
            };
            medical.ServiceDetailts = new List<ServiceDetailts>();
            var Service = request.Service.Select(x => new ServiceDetailts()
            {
                ServicesId = x.ServiceId,
                Qty = x.Qty,
                Price = x.Price.Value,
                TotalAmount = x.Qty * x.Price.Value
            });
            medical.ServiceDetailts.AddRange(Service);
            if (request.Medicine != null)
            {
                medical.MedicineDetailts = new List<MedicineDetailts>();
                var medicine = request.Medicine.Select(x => new MedicineDetailts()
                {
                    MedicineId = x.MedicineId,
                    Qty = x.Qty,
                    Afternoon = x.Afternoon==null?0:x.Afternoon.Value,
                    Noon = x.Noon==null?0:x.Noon.Value,
                    Night = x.Night==null?0:x.Night.Value,
                    Morning = x.Morning==null?0:x.Morning.Value,
                    Use = x.Use,
                    Price = x.Price.Value,
                    TotalAmount = x.Qty * x.Price.Value
                });
                medical.MedicineDetailts.AddRange(medicine);
            }
            _context.MedicalRecords.Add(medical);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var appointment = await _context.Appointments.FindAsync(request.AppointmentId);
                appointment.Status = StatusAppointment.complete;
                await _context.SaveChangesAsync();
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Tạo hồ sơ bệnh không thành công!");
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(Id);
            int check = 0;
            if (medicalRecord == null) return new ApiSuccessResult<int>(check);
            if (medicalRecord.Status == Status.Active)
            {
                medicalRecord.Status = Status.InActive;
                check = 1;
            }
            else
            {
                _context.MedicalRecords.Remove(medicalRecord);
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<MedicalRecordVm>>> GetAll()
        {
            var query = _context.MedicalRecords;

            var rs = await query.Select(x => new MedicalRecordVm()
            {
                Id = x.Id,
                CreateAt = x.CreatedAt,
                AppointmentId = x.AppointmentId,
                Diagnose = x.Diagnose,
                Note = x.Note,
                StatusIllness = x.StatusIllness,
                Status = x.Status
            }).ToListAsync();
            return new ApiSuccessResult<List<MedicalRecordVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<MedicalRecordVm>>> GetAllPaging(GetMedicalRecordPagingRequest request)
        {
            var query = from c in _context.MedicalRecords select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.CreatedAt.ToShortDateString().Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MedicalRecordVm()
                {
                    Id = x.Id,
                    CreateAt = x.CreatedAt,
                    AppointmentId = x.AppointmentId,
                    Diagnose = x.Diagnose,
                    Note = x.Note,
                    StatusIllness = x.StatusIllness,
                    Status = x.Status

                }).ToListAsync();

            var pagedResult = new PagedResult<MedicalRecordVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<MedicalRecordVm>>(pagedResult);
        }

        public async Task<ApiResult<MedicalRecordVm>> GetById(Guid Id)
        {
            var medicalRecords = await _context.MedicalRecords.FindAsync(Id);
            if (medicalRecords == null) new ApiErrorResult<MedicalRecordVm>("Hồ sơ bệnh không được xác nhận!");
            var rs = new MedicalRecordVm()
            {
                Id = medicalRecords.Id,
                CreateAt = medicalRecords.CreatedAt,
                StatusIllness = medicalRecords.StatusIllness,
                AppointmentId = medicalRecords.AppointmentId,
                Diagnose = medicalRecords.Diagnose,
                Note = medicalRecords.Note,
                Status = medicalRecords.Status
            };
            return new ApiSuccessResult<MedicalRecordVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(MedicalRecordUpdateRequest request)
        {
            var medical = await _context.MedicalRecords.FindAsync(request.Id);
            if (medical == null) return new ApiErrorResult<bool>("Hồ sơ bệnh không được xác nhận!");

            medical.Status = request.Status;
            medical.Diagnose = request.Diagnose;
            medical.StatusIllness = request.StatusIllness;
            medical.Note = request.Note;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Cập nhật hồ sơ bệnh không thành công!");
        }
    }
}
