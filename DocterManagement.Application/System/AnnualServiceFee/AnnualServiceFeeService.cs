using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Users;
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
                    if (item.CreatedAt.AddDays(365) < DateTime.Now)
                    {
                        var a = new AnnualServiceFees()
                        {
                            Status = Data.Enums.StatusAppointment.pending,
                            CreatedAt = DateTime.Now,
                            DoctorId = item.DoctorId,
                            NeedToPay = 2400000,
                            TuitionPaidFreeNumBer=0,
                            Contingency = -2400000,
                            Type = "Chưa Nộp",
                            PaidDate = new DateTime(),
                        };
                        annualServiceFees.Add(a);
                    }
                }
                check = check+","+ item.DoctorId.ToString();
            }
            await _context.AnnualServiceFees.AddRangeAsync(annualServiceFees);
            await _context.SaveChangesAsync();

            var query = from c in _context.AnnualServiceFees 
                        join d in _context.Doctors on c.DoctorId equals d.UserId
                        select new {c,d};
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.d.FirstName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x=>x.c.CreatedAt).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AnnualServiceFeeVm()
                {
                    
                    Id = x.c.Id,
                    Status = x.c.Status,
                    AccountBank = x.c.AccountBank,
                    Contingency = x.c.Contingency,
                    CreatedAt = x.c.CreatedAt,
                    Image = x.c.Image,
                    NeedToPay = x.c.NeedToPay,
                    Note = x.c.Note,
                    PaidDate = x.c.PaidDate,
                    TransactionCode = x.c.TransactionCode,
                    TuitionPaidFreeNumBer = x.c.TuitionPaidFreeNumBer,
                    TuitionPaidFreeText = x.c.TuitionPaidFreeText,
                    Type = x.c.Type,
                    Doctor = new DoctorVm()
                    {
                        No = x.d.No,
                        FullName = x.d.LastName +" "+x.d.FirstName,
                        FirstName = x.d.FirstName,
                        LastName = x.d.LastName,
                        UserId = x.d.UserId,
                        Img = "user-content/" + x.d.Img,
                        User = new UserVm()
                        {
                            PhoneNumber = x.d.AppUsers.PhoneNumber,
                            Email = x.d.AppUsers.Email,
                        }
                    }
                
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

        public async Task<ApiResult<AnnualServiceFeeVm>> GetById(Guid Id)
        {
            var service = await _context.AnnualServiceFees.FindAsync(Id);
            if (service == null) return new ApiErrorResult<AnnualServiceFeeVm>("null");
            var doctor = await _context.Doctors.FindAsync(service.DoctorId);
            var user = await _context.AppUsers.FindAsync(service.DoctorId);
            var rs = new AnnualServiceFeeVm()
            {
                Id = service.Id,
                Status = service.Status,
                AccountBank = service.AccountBank,
                Contingency = service.Contingency,
                CreatedAt = service.CreatedAt,
                Image = service.Image,
                NeedToPay = service.NeedToPay,
                Note = service.Note,
                PaidDate = service.PaidDate,
                TransactionCode = service.TransactionCode,
                TuitionPaidFreeNumBer = service.TuitionPaidFreeNumBer,
                TuitionPaidFreeText = service.TuitionPaidFreeText,
                Type = service.Type,
                Doctor = new DoctorVm()
                {
                    No = doctor.No,
                    FullName = doctor.LastName + " " + doctor.FirstName,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    UserId = doctor.UserId,
                    Img = "user-content/" + doctor.Img,
                    User = new UserVm()
                    {
                        PhoneNumber = doctor.AppUsers.PhoneNumber,
                        Email = doctor.AppUsers.Email,
                    }
                }
            };
            return new ApiSuccessResult<AnnualServiceFeeVm>(rs);
        }
    }
}
