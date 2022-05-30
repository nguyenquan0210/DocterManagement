using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public class StatisticApiClient : BaseApiClient, IStatisticApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StatisticApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<StatisticActive>> GetServiceFeeStatiticYear(GetHistoryActivePagingRequest request)
        {
            var data = await GetListAsync<HistoryActiveVm>(
                $"/api/statistic/all?keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&role={request.role}");
            var fromdate = DateTime.Parse("01/01/" + request.year);
            List<StatisticActive> model = new List<StatisticActive>();

            for (var i = 1; i <= 12; i++)
            {
                var his = data.Data.Where(x => x.CreatedAt.ToString("MM/yyyy") == fromdate.ToString("MM/yyyy")).ToList();
                decimal duration = 0;
                foreach (var item in his) duration = duration + item.HistoryActiveDetailts.Sum(x => x.ExecutionDuration);

                duration = decimal.Parse((duration / 3600).ToString("F"));
                model.Add(new StatisticActive
                {
                    date = i == 1 ? "thg " + fromdate.ToString("MM/yyyy") : "thg " + fromdate.ToString("MM"),
                    qty = data.Data.Where(x => x.CreatedAt.ToString("MM/yyyy") == fromdate.ToString("MM/yyyy")).Sum(x => x.Qty),
                    count = data.Data.Count(x => x.CreatedAt.ToString("MM/yyyy") == fromdate.ToString("MM/yyyy")),
                    duration = duration
                });
                fromdate = fromdate.AddMonths(1);
            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<List<StatisticActive>> GetServiceFeeStatiticDay(GetHistoryActivePagingRequest request)
        {
            var data = await GetListAsync<HistoryActiveVm>(
                $"/api/statistic/all?keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&role={request.role}");
            var fromdate = DateTime.Parse(request.day + "/" + request.month + "/" + request.year);

            List<StatisticActive> model = new List<StatisticActive>();

            for (var i = 1; i <= 24; i++)
            {
                var his = data.Data.Where(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == fromdate.ToString("dd/MM/yyyy HH")).ToList();
                decimal duration = 0;
                foreach (var item in his) duration = duration + item.HistoryActiveDetailts.Sum(x => x.ExecutionDuration);
                duration = decimal.Parse((duration / 3600).ToString("F"));
                model.Add(new StatisticActive
                {
                    date = i == 1 ? fromdate.ToString("HH dd/MM/yyyy") : fromdate.ToString("HH") + "h",
                    qty = data.Data.Where(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == fromdate.ToString("dd/MM/yyyy HH")).Sum(x => x.Qty),
                    count = data.Data.Count(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == fromdate.ToString("dd/MM/yyyy HH")),
                    duration = duration
                });
                fromdate = fromdate.AddHours(1);
            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<List<StatisticActive>> GetServiceFeeStatiticMonth(GetHistoryActivePagingRequest request)
        {
            var data = await GetListAsync<HistoryActiveVm>(
                $"/api/statistic/all?keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&role={request.role}");
            var fromdate = DateTime.Parse("01/" + request.month + "/" + request.year);
            List<StatisticActive> model = new List<StatisticActive>();

            for (var i = 1; i <= 31; i++)
            {
                decimal duration = 0;
                if (fromdate.ToString("MM") == request.month)
                {
                    var his = data.Data.Where(x => x.CreatedAt.ToString("dd/MM/yyyy") == fromdate.ToString("dd/MM/yyyy")).ToList();
                    
                    foreach (var item in his) duration = duration + item.HistoryActiveDetailts.Sum(x => x.ExecutionDuration);
                    duration = decimal.Parse((duration / 3600).ToString("F"));
                    model.Add(new StatisticActive
                    {
                        date = i == 1 ? "Ng " + fromdate.ToString("dd/MM/yyyy") : "Ng " + fromdate.ToString("dd"),
                        qty = data.Data.Where(x => x.CreatedAt.ToString("dd/MM/yyyy") == fromdate.ToString("dd/MM/yyyy")).Sum(x => x.Qty),
                        count = data.Data.Count(x => x.CreatedAt.ToString("dd/MM/yyyy") == fromdate.ToString("dd/MM/yyyy")),
                        duration = duration
                    });
                }
                fromdate = fromdate.AddDays(1);
            }
            return model.OrderBy(x => x.date).ToList();
        }
    }
}
