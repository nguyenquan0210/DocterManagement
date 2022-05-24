using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.AnnualServiceFee
{
    public class AnnualServiceFeeService : IAnnualServiceFeeService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private const string AnnualServiceFee_CONTENT_FOLDER_NAME = "annualservicefee-content";
        public AnnualServiceFeeService(DoctorManageDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<ApiResult<PagedResult<AnnualServiceFeeVm>>> GetAllPaging(GetAnnualServiceFeePagingRequest request)
        {
            var list = await _context.AnnualServiceFees.OrderByDescending(x=>x.CreatedAt).ToListAsync();
            var check = ",";
            var doctor = from d in _context.Doctors select d;
            var annualServiceFees = new List<AnnualServiceFees>();
            foreach(var item in list)
            {
                if (!check.Contains(item.DoctorId.ToString()))
                {
                    if (item.CreatedAt.AddDays(365) > DateTime.Now)
                    {
                        var a = new AnnualServiceFees()
                        {
                            Status = Data.Enums.StatusAppointment.pending,
                            CreatedAt = DateTime.Now,
                            DoctorId = item.DoctorId,
                            NeedToPay = 2400000,
                            Type = "Chưa Nộp",
                            PaidDate = DateTime.Now,
                        };
                        annualServiceFees.Add(a);
                    }
                }
                check = check+","+ item.DoctorId.ToString();
            }
            await _context.AnnualServiceFees.AddRangeAsync(annualServiceFees);
            await _context.SaveChangesAsync();

            var query = from c in _context.AnnualServiceFees  select c;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Image.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x=>x.CreatedAt).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AnnualServiceFeeVm()
                {
                  
                    Id = x.Id,
                
                }).ToListAsync();

            var pagedResult = new PagedResult<AnnualServiceFeeVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<AnnualServiceFeeVm>>(pagedResult);
        }
    }
}
