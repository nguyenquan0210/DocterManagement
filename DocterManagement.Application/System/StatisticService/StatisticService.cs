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

        public async Task<ApiResult<List<HistoryActiveVm>>> ListActiveUser()
        {
            var query = _context.HistoryActives;
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
