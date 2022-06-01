using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Service;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Servicce
{
    public class ServicesService : IServicesService
    {
        private readonly DoctorManageDbContext _context;
        public ServicesService(DoctorManageDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> Create(ServiceCreateRequest request)
        {
            var Service = new Services()
            {
                Description = request.Description,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                ServiceName = request.ServiceName,
                Price = request.Price,
                DoctorId = request.DoctorId,
                Unit = request.Unit
            };
            _context.Services.Add(Service);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("");
        }
       
        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var Services = await _context.Services.FindAsync(Id);
            int check = 0;
            if (Services == null) return new ApiSuccessResult<int>(check);

            Services.IsDeleted = true;
            check = 2;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<ServiceVm>>> GetAll(string UserName)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == UserName);
            var query = _context.Services.Where(x => x.IsDeleted == false && x.DoctorId == user.Id);

            var rs = await query.Select(x => new ServiceVm()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Price = x.Price,
                IsDeleted = x.IsDeleted,
                ServiceName = x.ServiceName,
                DoctorId = x.DoctorId,
                Description = x.Description,
                Unit = x.Unit
            }).ToListAsync();
            return new ApiSuccessResult<List<ServiceVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<ServiceVm>>> GetAllPaging(GetServicePagingRequest request)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            var query = from m in _context.Services
                        where m.DoctorId == user.Id
                        select m;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.ServiceName.Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ServiceVm()
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Price = x.Price,
                    IsDeleted = x.IsDeleted,
                    ServiceName = x.ServiceName,
                    DoctorId = x.DoctorId,
                    Description = x.Description,
                    Unit = x.Unit
                }).ToListAsync();

            var pagedResult = new PagedResult<ServiceVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<ServiceVm>>(pagedResult);
        }

        public async Task<ApiResult<ServiceVm>> GetById(Guid Id)
        {
            var x = await _context.Services.FindAsync(Id);
            if (x == null) return new ApiErrorResult<ServiceVm>("null");
            var rs = new ServiceVm()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Price = x.Price,
                IsDeleted = x.IsDeleted,
                ServiceName = x.ServiceName,
                DoctorId = x.DoctorId,
                Description = x.Description,
                Unit = x.Unit
            };
            return new ApiSuccessResult<ServiceVm>(rs);
        }

        public async Task<ApiResult<bool>> Update(ServiceUpdateRequest request)
        {
            var Services = await _context.Services.FindAsync(request.Id);
            if (Services == null) return new ApiErrorResult<bool>("");

            Services.Description = request.Description;
            Services.ServiceName = request.ServiceName;
            Services.Price = request.Price;
            Services.IsDeleted = request.IsDeleted;
            Services.Unit = request.Unit;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("");
        }
    }
}
