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
        public async Task<ApiResult<Clinics>> Create(ClinicCreateRequest request)
        {
            string year = DateTime.Now.ToString("yy");
            int count = await _context.Clinics.Where(x => x.No.Contains("PK-" + year)).CountAsync();
            string str = "";
            if (count < 9) str = "PK-" + DateTime.Now.ToString("yy") + "-00" + (count + 1);
            else if (count < 99) str = "PK-" + DateTime.Now.ToString("yy") + "-0" + (count + 1);
            else if (count < 999) str = "PK-" + DateTime.Now.ToString("yy") + "-" + (count + 1);
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
            return new ApiSuccessResult<Clinics>(clinics);
        }

        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var clinics = await _context.Clinics.FindAsync(Id);
            int check = 0;
            if (clinics == null) return new ApiSuccessResult<int>(check);

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
            return new ApiSuccessResult<int>(check); ;
        }

        public async Task<ApiResult<List<ClinicVm>>> GetAll()
        {
            var query = _context.Clinics;

            var rs =  await query.Select(x => new ClinicVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                WardId = x.WardId,
                ImgLogo = x.ImgLogo,
                Address = x.Address,
                Status = x.Status
            }).ToListAsync();
            return new ApiSuccessResult<List<ClinicVm>>(rs); ;
        }

        public async Task<ApiResult<PagedResult<ClinicVm>>> GetAllPaging(GetClinicPagingRequest request)
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
                    Address = x.Address,
                    WardId = x.WardId
                }).ToListAsync();

            var pagedResult = new PagedResult<ClinicVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return  new ApiSuccessResult<PagedResult<ClinicVm>>(pagedResult); 
        }

        public async Task<ApiResult<ClinicVm>> GetById(Guid Id)
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

            return new ApiSuccessResult<ClinicVm>(rs);
        }

        public async Task<ApiResult<Clinics>> Update(ClinicUpdateRequest request)
        {
            var clinics = await _context.Clinics.FindAsync(request.Id);
            if (clinics == null) throw new DoctorManageException($"Cannot find a Clinic with id: { request.Id}");
            clinics.Name = request.Name;
            clinics.Description = request.Description;
            clinics.Address = request.Address;
            clinics.WardId = request.WardId;
            clinics.Status = request.Status;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Clinics>(clinics);
        }
    }
}
