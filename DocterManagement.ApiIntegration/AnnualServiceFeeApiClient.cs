using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public class AnnualServiceFeeApiClient : BaseApiClient, IAnnualServiceFeeApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnnualServiceFeeApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<bool>> ApprovedServiceFee(Guid Id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);


            var response = await client.GetAsync($"/api/annualServiceFee/approved-service-fee/{Id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> CanceledServiceFee(AnnualServiceFeeCancelRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/annualServiceFee/canceled-service-fee", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<PagedResult<AnnualServiceFeeVm>>> GetAllPaging(GetAnnualServiceFeePagingRequest request)
        {
            return await GetAsync<PagedResult<AnnualServiceFeeVm>>(
               $"/api/annualServiceFee/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                $"&keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&status={request.status}");
        }
        public async Task<ApiResult<AnnualServiceFeeVm>> GetById(Guid Id)
        {
            return await GetAsync<AnnualServiceFeeVm>($"/api/annualServiceFee/{Id}");
        }

        public async Task<List<StatisticNews>> GetServiceFeeStatiticYear(GetAnnualServiceFeePagingRequest request)
        {
            var data = await GetAsync<PagedResult<AnnualServiceFeeVm>>(
                $"/api/annualServiceFee/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                $"&keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&status={request.status}");
            var datenews = data.Data.Items.Where(x => x.CreatedAt.ToString("yyyy") == request.year).Select(x => x.CreatedAt.ToString("MM/yyyy")).Distinct();
            List<StatisticNews> model = new List<StatisticNews>();

            foreach (var item in datenews)
            {
                model.Add(new StatisticNews
                {
                    date = item,
                    amount = (data.Data.Items.Where(x => x.CreatedAt.ToString("MM/yyyy") == item).Sum(x=>x.TuitionPaidFreeNumBer))/1000000 ,
                    count = data.Data.Items.Count(x => x.CreatedAt.ToString("MM/yyyy") == item)
                });

            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<List<StatisticNews>> GetServiceFeeStatiticDay(GetAnnualServiceFeePagingRequest request)
        {
            var data = await GetAsync<PagedResult<AnnualServiceFeeVm>>(
                $"/api/annualServiceFee/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                $"&keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&status={request.status}");
            var datenews = data.Data.Items.Where(x => x.CreatedAt.ToString("dd/MM/yyyy") == request.day + "/" + request. month + "/" + request.year).Select(x => x.CreatedAt.ToString("dd/MM/yyyy HH")).Distinct();
           
            List<StatisticNews> model = new List<StatisticNews>();

            foreach (var item in datenews)
            {
                model.Add(new StatisticNews
                {
                    date = item + "h",
                    amount = (data.Data.Items.Where(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == item).Sum(x => x.TuitionPaidFreeNumBer)) / 1000000,
                    count = data.Data.Items.Count(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == item)
                });

            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<List<StatisticNews>> GetServiceFeeStatiticMonth(GetAnnualServiceFeePagingRequest request)
        {
            var data = await GetAsync<PagedResult<AnnualServiceFeeVm>>(
                $"/api/annualServiceFee/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                $"&keyword={request.Keyword}" +
                $"&day={request.day}" +
                $"&month={request.month}" +
                $"&year={request.year}" +
                $"&status={request.status}");
            var datenews = data.Data.Items.Where(x => x.CreatedAt.ToString("MM/yyyy") == request.month + "/" + request.year).Select(x => x.CreatedAt.ToString("dd/MM/yyyy")).Distinct();

            List<StatisticNews> model = new List<StatisticNews>();

            foreach (var item in datenews)
            {
                model.Add(new StatisticNews
                {
                    date = item ,
                    amount = (data.Data.Items.Where(x => x.CreatedAt.ToString("dd/MM/yyyy") == item).Sum(x => x.TuitionPaidFreeNumBer)) / 1000000,
                    count = data.Data.Items.Count(x => x.CreatedAt.ToString("dd/MM/yyyy") == item)
                });

            }
            return model.OrderBy(x => x.date).ToList();
        }

        public async Task<ApiResult<bool>> PaymentServiceFee(AnnualServiceFeePaymentRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/annualServiceFee/payment-service-fee", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> PaymentServiceFeeDoctor(AnnualServiceFeePaymentDoctorRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/annualServiceFee/payment-service-fee-doctor", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }
    }
}
