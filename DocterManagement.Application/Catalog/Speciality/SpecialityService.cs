﻿using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Speciality
{
    public class SpecialityService : ISpecialityService
    {
        private readonly DoctorManageDbContext _context;

        public SpecialityService(DoctorManageDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(SpecialityCreateRequest request)
        {
            var specialities = new Specialities()
            {
                Title = request.Title,
                SortOrder = request.SortOrder,
                Description = request.Description
            };
            _context.Specialities.Add(specialities);
            await _context.SaveChangesAsync();
            return specialities.Id;
        }

        public async Task<int> Delete(Guid Id)
        {
            var speciality = await _context.Specialities.FindAsync(Id);
            int check = 0;
            if (speciality == null) return check;
            if (speciality.Status == Status.Active)
            {
                speciality.Status = Status.InActive;
                check = 1;
            }
            else
            {
                _context.Specialities.Remove(speciality);
                check = 2;
            }
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<List<SpecialityVm>> GetAll()
        {
            var query = _context.Specialities.Where(x => x.Status == Status.Active);

            return await query.Select(x => new SpecialityVm()
            {
                Id = x.Id,
                Title = x.Title,
                SortOrder = x.SortOrder,
                Status = x.Status
            }).ToListAsync();
        }

        public async Task<PagedResult<SpecialityVm>> GetAllPaging(GetSpecialityPagingRequest request)
        {
            var query = from c in _context.Specialities select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Title.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SpecialityVm()
                {
                    Title = x.Title,
                    SortOrder = x.SortOrder,
                    Id = x.Id,
                    Status = x.Status

                }).ToListAsync();

            var pagedResult = new PagedResult<SpecialityVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<SpecialityVm> GetById(int Id)
        {
            var speciality = await _context.Specialities.FindAsync(Id);

            var rs = new SpecialityVm()
            {
                Id = speciality.Id,
                Title = speciality.Title,
                SortOrder = speciality.SortOrder,
                Status = speciality.Status
            };

            return rs;
        }

        public async Task<int> Update(SpecialityUpdateRequest request)
        {
            var speciality = await _context.Specialities.FindAsync(request.Id);
            if (speciality == null) throw new DoctorManageException($"Cannot find a specialitygory with id: { request.Id}");
            speciality.Title = request.Title;
            speciality.SortOrder = request.SortOrder;
            speciality.Description = request.Description;
            speciality.Status = request.Status ? Status.Active : Status.InActive;

            return await _context.SaveChangesAsync();
        }
    }
}