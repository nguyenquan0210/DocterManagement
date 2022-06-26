using DoctorManagement.Application.Common;
using DoctorManagement.Application.System.Users;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Models;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.AnnualServiceFee
{
    public class AnnualServiceFeeService : IAnnualServiceFeeService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IEmailService _emailService;
        private const string ANNUALSERVICEFEE_CONTENT_FOLDER_NAME = "annualservicefee-content";
        public AnnualServiceFeeService(DoctorManageDbContext context, IStorageService storageService,
            IEmailService emailService)
        {
            _context = context;
            _storageService = storageService;
            _emailService = emailService;
        }
        public async Task<ApiResult<PagedResult<AnnualServiceFeeVm>>> GetAllPaging(GetAnnualServiceFeePagingRequest request)
        {
            var list = await _context.AnnualServiceFees.OrderByDescending(x=>x.CreatedAt).ToListAsync();
            var check = ",";
            var doctor = from d in _context.Doctors select d;
            var annualServiceFees = new List<AnnualServiceFees>();
            var information = await _context.Informations.FirstOrDefaultAsync();
            string day = DateTime.Now.ToString("dd") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("YY");
            int count = await _context.AnnualServiceFees.Where(x => x.No.Contains("DMPM" + day)).CountAsync();
            string str = "";
            var users = new List<AppUsers>();    
            foreach (var item in list)
            {
                if (!check.Contains(item.DoctorId.ToString()))
                {
                    if (item.CreatedAt.AddDays(365) < DateTime.Now)
                    {
                        if (count < 9) str = "DMPM" + day + "00000" + (count + 1);
                        else if (count < 99) str = "DMPM" + day + "0000" + (count + 1);
                        else if (count < 999) str = "DMPM" + day + "000" + (count + 1);
                        else if (count < 9999) str = "DMPM" + day + "00" + (count + 1);
                        else if (count < 99999) str = "DMPM" + day + "0" + (count + 1);
                        else if (count < 999999) str = "DMPM" + day + (count + 1);
                        var a = new AnnualServiceFees()
                        {
                            Status = item.Contingency > information.ServiceFee?StatusAppointment.complete: StatusAppointment.pending,
                            CreatedAt = item.CreatedAt.AddDays(366),
                            DoctorId = item.DoctorId,
                            NeedToPay = item.Contingency > information.ServiceFee? 0 : (information.ServiceFee - item.Contingency),
                            InitialAmount = information.ServiceFee,
                            TuitionPaidFreeNumBer=0,
                            Contingency = item.Contingency > information.ServiceFee?item.Contingency - information.ServiceFee:0,
                            Type = item.Contingency > information.ServiceFee ? item.Type:"Chưa Nộp",
                            PaidDate = new DateTime(),
                            No = str
                        };
                        annualServiceFees.Add(a);
                        count++;
                        var adduser = await _context.AppUsers.FindAsync(item.DoctorId);
                        users.Add(adduser);
                        if (item.CreatedAt.AddDays(365) < DateTime.Now.AddMonths(-3))
                        {
                            adduser.Status = Status.InActive;
                        }
                    }
                    check = check+ item.DoctorId.ToString()+",";
                }
                
            }
            await _context.AnnualServiceFees.AddRangeAsync(annualServiceFees);
            var rs = await _context.SaveChangesAsync();
            if(rs != 0)
            { var msg = "Nộp phí trễ quá 3 tháng tài khoản của bạn sẽ bị khóa!";
                await SendEmailServiceFee(users, msg, annualServiceFees.FirstOrDefault(),information);
            }
            var query = from c in _context.AnnualServiceFees 
                        join d in _context.Doctors on c.DoctorId equals d.UserId
                        select new {c,d};
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.d.FirstName.Contains(request.Keyword));
            }
            if (request.status != null)
            {
                query = query.Where(x => x.c.Status == request.status);
            }
            if (!string.IsNullOrEmpty(request.day))
            {
                var fromdate = DateTime.Parse(request.day + "/" + request.month + "/" +request.year);
                var todate = fromdate.AddDays(1);
                query = query.Where(x => x.c.CreatedAt>= fromdate && x.c.CreatedAt <= todate);
            }
            else if (!string.IsNullOrEmpty(request.month))
            {
                var fromdate = DateTime.Parse("01/" + request.month + "/" + request.year);
                var todate = fromdate.AddMonths(1);
                query = query.Where(x => x.c.CreatedAt >= fromdate && x.c.CreatedAt <= todate);
            }
            else if(!string.IsNullOrEmpty(request.year))
            {
                var fromdate = DateTime.Parse("01/01/" + request.year);
                var todate = fromdate.AddYears(1);
                query = query.Where(x => x.c.CreatedAt >= fromdate && x.c.CreatedAt <= todate);
            }
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.d.AppUsers.UserName == request.UserName);
            }
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x=>x.c.CreatedAt).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AnnualServiceFeeVm()
                {
                    
                    Id = x.c.Id,
                    No = x.c.No,
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
            var information = await _context.Informations.FirstOrDefaultAsync();
            var rs = new AnnualServiceFeeVm()
            {
                No = service.No,
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
                InitialAmount = service.InitialAmount,
                Type = service.Type,
                CancelReason = service.CancelReason,
                Information = new InformationVm()
                {
                    Company = information.Company,
                    Email = information.Email,
                    FullAddress = information.FullAddress,
                    Hotline = information.Hotline,
                    Image = information.Image,
                    TimeWorking = information.TimeWorking,
                    AccountBank = information.AccountBank,
                    AccountBankName = information.AccountBankName,
                    ServiceFee = information.ServiceFee,
                    Content = information.Content,
                },
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

        public  async Task<ApiResult<bool>> CanceledServiceFee(AnnualServiceFeeCancelRequest request)
        {
            var service = await _context.AnnualServiceFees.FindAsync(request.Id);
            var information = await _context.Informations.FirstOrDefaultAsync();
            if (service == null) return new ApiErrorResult<bool>("Dịch vụ nộp phí không được xác nhận!");
            service.Status = StatusAppointment.cancel;
            service.CancelReason = request.CancelReason;
            string day = DateTime.Now.ToString("dd") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("YY");
            int count = await _context.AnnualServiceFees.Where(x => x.No.Contains("DMPM" + day)).CountAsync();
            string str = "";
            if (count < 9) str = "DMPM" + day + "00000" + (count + 1);
            else if (count < 99) str = "DMPM" + day + "0000" + (count + 1);
            else if (count < 999) str = "DMPM" + day + "000" + (count + 1);
            else if (count < 9999) str = "DMPM" + day + "00" + (count + 1);
            else if (count < 99999) str = "DMPM" + day + "0" + (count + 1);
            else if (count < 999999) str = "DMPM" + day + (count + 1);
            var serviceFees = new AnnualServiceFees()
            {
                Status = StatusAppointment.pending,
                CreatedAt = service.CreatedAt.AddDays(1),
                DoctorId = service.DoctorId,
                NeedToPay = (service.TuitionPaidFreeNumBer - service.Contingency) > 0 ? service.TuitionPaidFreeNumBer - service.Contingency : 0,
                Contingency = (service.TuitionPaidFreeNumBer - service.Contingency)>0?0: service.Contingency - service.TuitionPaidFreeNumBer,
                TuitionPaidFreeNumBer = 0,
                InitialAmount = service.InitialAmount,
                TuitionPaidFreeText = "0 VN đồng.",
                Type = "Chưa Nộp",
                PaidDate = new DateTime(),
                No = str
            };
            await _context.AnnualServiceFees.AddAsync(serviceFees);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var users = new List<AppUsers>();
                users.Add(await _context.AppUsers.FindAsync(service.DoctorId));
                var msg = "Nộp phí của bạn có sai sốt yêu cầu điền lại đầy đủ thông tin";
                await SendEmailServiceFee(users, msg, serviceFees, information);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Hủy dịch vụ đóng phí không thành công!");
        }

        public async Task<ApiResult<bool>> ApprovedServiceFee(Guid Id)
        {
            var service = await _context.AnnualServiceFees.FindAsync(Id);
            if (service == null) return new ApiErrorResult<bool>("Dịch cụ nộp phí không được xác nhận!");
            /*var serviceFees = await _context.AnnualServiceFees.Where(x => x.DoctorId == service.DoctorId && x.CreatedAt > service.CreatedAt).OrderByDescending(x => x.CreatedAt).ToListAsync();
            foreach(var remove in serviceFees)
            {
                var removeservice = await _context.AnnualServiceFees.FindAsync(remove.Id);
                _context.AnnualServiceFees.Remove(removeservice);
            }*/
            service.Status = StatusAppointment.complete;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var information = await _context.Informations.FirstOrDefaultAsync();
                var users = new List<AppUsers>();
                users.Add(await _context.AppUsers.FindAsync(service.DoctorId));
                var msg = "Nộp phí của bạn đã được người quản trị duyệt";
                await SendEmailServiceFee(users, msg, service, information);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Duyệt dịch vụ nộp phí không thành công!");
        }

        public async Task<ApiResult<bool>> PaymentServiceFee(AnnualServiceFeePaymentRequest request)
        {
            var service = await _context.AnnualServiceFees.FindAsync(request.Id);
            if (service == null) return new ApiErrorResult<bool>("Dịch cụ nộp phí không được xác nhận!");
            service.Status = StatusAppointment.complete;
            service.Note = request.Note;
            service.PaidDate = DateTime.Now;
            service.TuitionPaidFreeNumBer = request.TuitionPaidFreeNumBer;
            service.TuitionPaidFreeText = request.TuitionPaidFreeText;
            service.Type = "trực tiếp";
            service.Contingency =  service.Contingency  +  (request.TuitionPaidFreeNumBer > service.NeedToPay?(request.TuitionPaidFreeNumBer - service.NeedToPay):0) ;
            service.NeedToPay = request.TuitionPaidFreeNumBer > service.NeedToPay ? 0 : (service.NeedToPay - request.TuitionPaidFreeNumBer);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var information = await _context.Informations.FirstOrDefaultAsync();
                var users = new List<AppUsers>();
                users.Add(await _context.AppUsers.FindAsync(service.DoctorId));
                var msg = "Nộp phí thành công tại cơ sở";
                await SendEmailServiceFee(users, msg, service, information);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Nộp phí dịch vụ không thành công!");
        }
        public async Task<ApiResult<bool>> PaymentServiceFeeDoctor(AnnualServiceFeePaymentDoctorRequest request)
        {
            var service = await _context.AnnualServiceFees.FindAsync(request.Id);
            if (service == null) return new ApiErrorResult<bool>("Dịch cụ nộp phí không được xác nhận!");
            service.Status = StatusAppointment.approved;
            service.Note = request.Note;
            service.PaidDate = DateTime.Now;
            service.TuitionPaidFreeNumBer = request.TuitionPaidFreeNumBer;
            service.TuitionPaidFreeText = request.TuitionPaidFreeText;
            service.TransactionCode = request.TransactionCode;
            service.BankName = request.BankName;
            service.AccountBank = request.AccountBank;
            service.Image = await SaveFile(request.Image, ANNUALSERVICEFEE_CONTENT_FOLDER_NAME);
            service.Type = "trực tuyến";
            service.Contingency = service.Contingency + (request.TuitionPaidFreeNumBer > service.NeedToPay ? (request.TuitionPaidFreeNumBer - service.NeedToPay) : 0);
            service.NeedToPay = request.TuitionPaidFreeNumBer > service.NeedToPay ? 0 : (service.NeedToPay - request.TuitionPaidFreeNumBer);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                var information = await _context.Informations.FirstOrDefaultAsync();
                var users = new List<AppUsers>();
                users.Add(await _context.AppUsers.FindAsync(service.DoctorId));
                var msg = "Nộp phí chuyển khoản và đợi người quản trị duyệt";
                await SendEmailServiceFee(users, msg, service, information);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Nộp phí dịch vụ không thành công!");
        }
        private async Task<string> SaveFile(IFormFile? file, string folderName)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsyncs(file.OpenReadStream(), fileName, folderName);
            return fileName;
        }

        private async Task SendEmailServiceFee(List<AppUsers> users, string messenger, AnnualServiceFees serviceFee, Informations informations)
        {
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = users.Select(x=>  x.Email ).ToList(),
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{NeedToPay}}", serviceFee.InitialAmount.ToString("#,###,###,### vnđ")),
                    new KeyValuePair<string, string>("{{Messenger}}", messenger),
                    new KeyValuePair<string, string>("{{Hotline}}", informations.Hotline),
                    new KeyValuePair<string, string>("{{AccountBank}}", informations.AccountBank),
                    new KeyValuePair<string, string>("{{AccountBankName}}", informations.AccountBankName),
                    new KeyValuePair<string, string>("{{FullAddress}}", informations.FullAddress),
                }
            };
            await _emailService.SendEmailServiceFee(options);
        }
    }
}
