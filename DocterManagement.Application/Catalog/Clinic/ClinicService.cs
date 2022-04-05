using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Clinic
{
    public class ClinicService : IClinicService
    {
        private readonly DoctorManageDbContext _context;

        public ClinicService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(ClinicCreateRequest request)
        {
            string year = DateTime.Now.ToString("yy");
            int count = await _context.Clinics.Where(x => x.No.Contains("PK-" + year)).CountAsync();
            string str = "";
            if (count < 10) str = "PK-" + DateTime.Now.ToString("yy") + "-00" + (count + 1);
            else if (count < 100) str = "PK-" + DateTime.Now.ToString("yy") + "-0" + (count + 1);
            else if (count < 1000) str = "PK-" + DateTime.Now.ToString("yy") + "-" + (count + 1);
            var clinics = new Clinics()
            {
                Name = request.Name,
                ImgLogo = request.ImgLogo,
                Description = request.Description,
                Address = request.Address,
                WardId = request.WardId,
                Status = Status.Active,
                No = str
            };
            _context.Clinics.Add(clinics);
            await _context.SaveChangesAsync();
            return  clinics.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var clinics = await _context.Clinics.FindAsync(Id);
            int check = 0;
            if (clinics == null) return check;

            if (clinics.Status == Status.Active)
            {
                clinics.Status = Status.InActive;
                check = 1;
            }
            else
            {
                _context.Clinics.Remove(clinics);
                check = 2;
            }

            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<ClinicVm>> GetAll()
        {
            var query = _context.Clinics;

            return await query.Select(x => new ClinicVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                WardId = x.WardId,
                ImgLogo = x.ImgLogo,
                Address = x.Address,
                Status = x.Status
            }).ToListAsync();
        }

        public async Task<PagedResult<ClinicVm>> GetAllPaging(GetClinicPagingRequest request)
        {
            var query = from c in _context.Clinics select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ClinicVm()
                {
                    Name = x.Name,
                    Description = x.Description,
                    Id = x.Id,
                    ImgLogo = x.ImgLogo,
                    Status = x.Status,
                    Address = x.Address

                }).ToListAsync();

            var pagedResult = new PagedResult<ClinicVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<ClinicVm> GetById(Guid Id)
        {
            var Clinics = await _context.Clinics.FindAsync(Id);
            if (Clinics == null) throw new DoctorManageException($"Cannot find a Clinic with id: { Id}");
            var rs = new ClinicVm()
            {
                Id = Clinics.Id,
                Name = Clinics.Name,
                Description = Clinics.Description,
                Address = Clinics.Address,
                Status = Clinics.Status
            };

            return rs;
        }

        public async Task<int> Update(ClinicUpdateRequest request)
        {
            var Clinics = await _context.Clinics.FindAsync(request.Id);
            if (Clinics == null) throw new DoctorManageException($"Cannot find a Clinic with id: { request.Id}");
            Clinics.Name = request.Name;
            Clinics.Description = request.Description;
            Clinics.Address = request.Address;
            Clinics.WardId = request.WardId;
            Clinics.Status = request.Status;

            return await _context.SaveChangesAsync();
        }
    }
}
