using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Statistic;
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
            var hiss = _context.HistoryActives.FirstOrDefault(x=>x.User == request.User&&x.CreatedAt>= fromdate && x.CreatedAt < fromdate.AddDays(1));
            
            if (hiss == null)
            {
                var his = new HistoryActives()
                {
                    CreatedAt = DateTime.Now,
                    Qty = 1,
                    Type = request.Type,
                    User = request.User,
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
                var his = await _context.HistoryActives.FindAsync(hiss.Id);
                his.Qty = his.Qty + 1;
                his.CreatedAt = request.FromTime;
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
            else
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
    }
}
