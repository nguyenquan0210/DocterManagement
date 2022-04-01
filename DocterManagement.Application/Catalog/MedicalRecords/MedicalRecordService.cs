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
        public async Task<Guid> Create(MedicalRecordCreateRequest request)
        {
            var MedicalRecords = new MedicalRecord()
            {
                Diagnose = request.Diagnose,
                Note = request.Note,
                StatusIllness = request.StatusIllness,
                Prescription = request.Prescription,
                Date = DateTime.Now,
                Status = Status.Active,
                PatientId = request.PatientId,
                AppointmentId = request.AppointmentId,
                DoctorId = request.DoctorId
            };
            _context.MedicalRecords.Add(MedicalRecords);
            await _context.SaveChangesAsync();
            return MedicalRecords.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(Id);
            int check = 0;
            if (medicalRecord == null) return check;
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
            return check;
        }

        public async Task<List<MedicalRecordVm>> GetAll()
        {
            var query = _context.MedicalRecords;

            return await query.Select(x => new MedicalRecordVm()
            {
                Id = x.Id,
                Date = x.Date,
                DoctorId= x.DoctorId,
                PatientId = x.PatientId,
                AppointmentId = x.AppointmentId,
                Diagnose = x.Diagnose,
                Note = x.Note,
                Prescription = x.Prescription,
                StatusIllness = x.StatusIllness,
                Status = x.Status
            }).ToListAsync();
        }

        public async Task<PagedResult<MedicalRecordVm>> GetAllPaging(GetMedicalRecordPagingRequest request)
        {
            var query = from c in _context.MedicalRecords select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Date.ToShortDateString().Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MedicalRecordVm()
                {
                    Id = x.Id,
                    Date = x.Date,
                    DoctorId = x.DoctorId,
                    PatientId = x.PatientId,
                    AppointmentId = x.AppointmentId,
                    Diagnose = x.Diagnose,
                    Note = x.Note,
                    Prescription = x.Prescription,
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
            return pagedResult;
        }

        public async Task<MedicalRecordVm> GetById(Guid Id)
        {
            var medicalRecords = await _context.MedicalRecords.FindAsync(Id);
            if (medicalRecords == null) throw new DoctorManageException($"Cannot find a MedicalRecord with id: { Id}");
            var rs = new MedicalRecordVm()
            {
                Id = medicalRecords.Id,
                Date = medicalRecords.Date,
                PatientId = medicalRecords.PatientId,
                DoctorId = medicalRecords.DoctorId,
                StatusIllness = medicalRecords.StatusIllness,
                AppointmentId = medicalRecords.AppointmentId,
                Diagnose = medicalRecords.Diagnose,
                Note = medicalRecords.Note,
                Prescription = medicalRecords.Prescription,
                Status = medicalRecords.Status
            };
            return rs;
        }

        public async Task<int> Update(MedicalRecordUpdateRequest request)
        {
            var medicalRecords = await _context.MedicalRecords.FindAsync(request.Id);
            if (medicalRecords == null) throw new DoctorManageException($"Cannot find a MedicalRecord with id: { request.Id}");

            medicalRecords.Status = request.Status;
            medicalRecords.Prescription = request.Prescription;
            medicalRecords.Diagnose = request.Diagnose;
            medicalRecords.StatusIllness = request.StatusIllness;
            medicalRecords.Note = request.Note;
            return await _context.SaveChangesAsync();
        }
    }
}
