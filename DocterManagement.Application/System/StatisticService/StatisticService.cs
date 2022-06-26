using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.StatisticService
{
    public class StatisticService : IStatisticService
    {
        private readonly DoctorManageDbContext _context;
        public StatisticService(DoctorManageDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> AddActiveUser(HistoryActiveCreateRequest request)
        {
            var timespan = request.ToTime - request.FromTime;
            var executionDuration = (int) timespan.TotalSeconds;
            var fromdate = DateTime.Parse(request.FromTime.ToShortDateString());
            
            var hiss = _context.HistoryActives.FirstOrDefault(x=>x.User == request.Usertemporary&&x.CreatedAt>= fromdate && x.CreatedAt < fromdate.AddDays(1));
            var hissemporary = new HistoryActives();
            if(hiss == null && request.User != null && request.Type!="doctor") hissemporary = _context.HistoryActives.FirstOrDefault(x=>x.User == request.User&&x.CreatedAt>= fromdate && x.CreatedAt < fromdate.AddDays(1));
            var user = request.User == null ? request.Usertemporary : request.User;
            if (hiss == null && hissemporary.User == null)
            {
                var his = new HistoryActives()
                {
                    CreatedAt = DateTime.Now,
                    Qty = 1,
                    Type = request.Type,
                    User = user,
                    HistoryActiveDetailts = new List<HistoryActiveDetailts>()
                };
                var hisd = new HistoryActiveDetailts()
                {
                    ServiceName = request.ServiceName,
                    ExecutionDuration = executionDuration,
                    ExecutionTime = request.FromTime,
                    ExtraProperties = request.ExtraProperties,
                    MethodName = request.MethodName,
                    Parameters = request.Parameters,
                };
                his.HistoryActiveDetailts.Add(hisd);
                await _context.HistoryActives.AddAsync(his);
            }
            else
            {
                var his = await _context.HistoryActives.FindAsync(hiss ==null?hissemporary.Id:hiss.Id);
                his.Qty = his.Qty + 1;
                his.CreatedAt = request.FromTime;
                his.User = user;
                his.HistoryActiveDetailts = new List<HistoryActiveDetailts>();
                var hisd = new HistoryActiveDetailts()
                {
                    ServiceName = request.ServiceName,
                    ExecutionDuration = executionDuration,
                    ExecutionTime = request.FromTime,
                    ExtraProperties = request.ExtraProperties,
                    MethodName = request.MethodName,
                    Parameters = request.Parameters,
                };
                his.HistoryActiveDetailts.Add(hisd);
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
            
        }

        public async Task<ApiResult<List<HistoryActiveVm>>> ListActiveUser(GetHistoryActivePagingRequest request)
        {
            var query = from h in _context.HistoryActives select h;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.User.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.role))
            {
                query = query.Where(x => x.Type.Contains(request.role));
            }
            if (!string.IsNullOrEmpty(request.day))
            {
                var fromdate = DateTime.Parse(request.day + "/" + request.month + "/" + request.year);
                var todate = fromdate.AddDays(1);
                query = query.Where(x => x.CreatedAt >= fromdate && x.CreatedAt <= todate);
            }
            else if (!string.IsNullOrEmpty(request.month))
            {
                var fromdate = DateTime.Parse("01/" + request.month + "/" + request.year);
                var todate = fromdate.AddMonths(1);
                query = query.Where(x => x.CreatedAt >= fromdate && x.CreatedAt <= todate);
            }
            else if(!string.IsNullOrEmpty(request.year))
            {
                var fromdate = DateTime.Parse("01/01/" + request.year);
                var todate = fromdate.AddYears(1);
                query = query.Where(x => x.CreatedAt >= fromdate && x.CreatedAt <= todate);
            }
            return new ApiSuccessResult<List<HistoryActiveVm>>(query.Select(x=> new HistoryActiveVm()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Qty = x.Qty,
                Type = x.Type,
                User = x.User,
                HistoryActiveDetailts = x.HistoryActiveDetailts.Select(s=>new HistoryActiveDetailtVm()
                {
                    Id=s.Id,
                    ServiceName = s.ServiceName,
                    ExecutionDuration = s.ExecutionDuration,
                    ExecutionTime = s.ExecutionTime,
                    ExtraProperties = s.ExtraProperties,
                    MethodName = s.MethodName,
                    Parameters = s.Parameters
                }).ToList(),
            }).ToList());
        }

        public async Task<ApiResult<List<HistoryActiveDetailtVm>>> ListActiveUserDetailt()
        {
            var query = from hd in _context.historyActiveDetailts select hd;
            return new ApiSuccessResult<List<HistoryActiveDetailtVm>>(await query.Select(s => new HistoryActiveDetailtVm()
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                ExecutionDuration = s.ExecutionDuration,
                ExecutionTime = s.ExecutionTime,
                ExtraProperties = s.ExtraProperties,
                MethodName = s.MethodName,
                Parameters = s.Parameters,
                Count = query.Where(x=>x.Parameters == s.Parameters).Count()
            }).ToListAsync());
        }
    }
}
