using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Patient;
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
    public class AppointmentApiClient : BaseApiClient, IAppointmentApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<string>> Create(AppointmentCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/appointment/", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(body);
        }
        public async Task<ApiResult<bool>> CreateMedicalRecord(MedicalRecordCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/medicalRecord/", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }
        public async Task<int> Delete(Guid Id)
        {
            return await Delete($"/api/appointment/" + Id);
        }

        public async Task<ApiResult<AppointmentVm>> GetById(Guid Id)
        {
            return await GetAsync<AppointmentVm>($"/api/appointment/{Id}");
        }

        public async Task<ApiResult<List<AppointmentVm>>> GetAllAppointment()
        {
            var data = await GetListAsync<AppointmentVm>($"/api/appointment/all");
            return data;
        }

        public async Task<ApiResult<PagedResult<AppointmentVm>>> GetAppointmentPagings(GetAppointmentPagingRequest request)
        {
            return await GetAsync<PagedResult<AppointmentVm>>(
               $"/api/appointment/paging?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}" +
               $"&userName={request.UserName}" +
               $"&userNameDoctor={request.UserNameDoctor}" +
               $"&status={request.status}" +
               $"&patientId={request.PatientId}");
        }
        public async Task<ApiResult<PagedResult<PatientVm>>> GetAppointmentPagingPatient(GetAppointmentPagingRequest request)
        {
            return await GetAsync<PagedResult<PatientVm>>(
               $"/api/appointment/paging-patient?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}" +
               $"&userName={request.UserName}" +
               $"&userNameDoctor={request.UserNameDoctor}" +
               $"&status={request.status}");
        }
        public async Task<ApiResult<PagedResult<AppointmentVm>>> GetAppointmentPagingRating(GetAppointmentPagingRequest request)
        {
            return await GetAsync<PagedResult<AppointmentVm>>(
               $"/api/appointment/paging-rating?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}" +
               $"&userName={request.UserName}" +
               $"&userNameDoctor={request.UserNameDoctor}" +
               $"&status={request.status}");
        }
        public async Task<List<StatisticNews>> GetAppointmentStatiticYear(GetAppointmentPagingRequest request)
        {
            var data = await GetAsync<PagedResult<AppointmentVm>>(
            $"/api/appointment/paging?pageIndex={1}" +
            $"&pageSize={1000000000}" +
            $"&keyword={request.Keyword}" +
            $"&userName={request.UserName}" +
            $"&userNameDoctor={request.UserNameDoctor}" +
            $"&status={request.status}" +
            $"&patientId={request.PatientId}" +
            $"&day={request.day}" +
            $"&month={request.month}" +
            $"&year={request.year}");
            var fromdate = DateTime.Parse("01/01/" + request.year);
            var datenews = data.Data.Items.Where(x => x.CreatedAt.ToString("yyyy") == request.year).Select(x => x.CreatedAt.ToString("MM/yyyy")).Distinct();
            List<StatisticNews> model = new List<StatisticNews>();

            for (var i = 1; i <= 12; i++)
            {
                model.Add(new StatisticNews
                {
                    date = i == 1 ? "thg " + fromdate.ToString("MM/yyyy") : "thg " + fromdate.ToString("MM"),
                    amount = data.Data.Items.DistinctBy(x => x.Patient.Id).Count(x => x.CreatedAt.ToString("MM/yyyy") == fromdate.ToString("MM/yyyy")),
                    count = data.Data.Items.Count(x => x.CreatedAt.ToString("MM/yyyy") == fromdate.ToString("MM/yyyy"))
                });
                fromdate = fromdate.AddMonths(1);
            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<List<StatisticNews>> GetAppointmentStatiticDay(GetAppointmentPagingRequest request)
        {
            var data = await GetAsync<PagedResult<AppointmentVm>>(
            $"/api/appointment/paging?pageIndex={1}" +
            $"&pageSize={1000000000}" +
            $"&keyword={request.Keyword}" +
            $"&userName={request.UserName}" +
            $"&userNameDoctor={request.UserNameDoctor}" +
            $"&status={request.status}" +
            $"&patientId={request.PatientId}" +
            $"&day={request.day}" +
            $"&month={request.month}" +
            $"&year={request.year}");
            var fromdate = DateTime.Parse(request.day + "/" + request.month + "/" + request.year);
            var datenews = data.Data.Items.Where(x => x.CreatedAt.ToString("dd/MM/yyyy") == request.day + "/" + request.month + "/" + request.year).Select(x => x.CreatedAt.ToString("dd/MM/yyyy HH")).Distinct();

            List<StatisticNews> model = new List<StatisticNews>();

            for (var i = 1; i <= 24; i++)
            {
                var date = datenews.FirstOrDefault(x => x == fromdate.ToString("dd/MM/yyyy HH"));
                model.Add(new StatisticNews
                {
                    date = i == 1 ? fromdate.ToString("HH dd/MM/yyyy ") : fromdate.ToString("HH") + "h",
                    countpatient = data.Data.Items.DistinctBy(x => x.Patient.Id).Count(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == fromdate.ToString("dd/MM/yyyy HH")),
                    count = data.Data.Items.Count(x => x.CreatedAt.ToString("dd/MM/yyyy HH") == fromdate.ToString("dd/MM/yyyy HH"))
                });
                fromdate = fromdate.AddHours(1);
            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<List<StatisticNews>> GetAppointmentStatiticMonth(GetAppointmentPagingRequest request)
        {
            var data = await GetAsync<PagedResult<AppointmentVm>>(
            $"/api/appointment/paging?pageIndex={1}" +
            $"&pageSize={1000000000}" +
            $"&keyword={request.Keyword}" +
            $"&userName={request.UserName}" +
            $"&userNameDoctor={request.UserNameDoctor}" +
            $"&status={request.status}" +
            $"&patientId={request.PatientId}" +
            $"&day={request.day}" +
            $"&month={request.month}" +
            $"&year={request.year}");
            var fromdate = DateTime.Parse("01/" + request.month + "/" + request.year);
            var datenews = data.Data.Items.Where(x => x.CreatedAt.ToString("MM/yyyy") == request.month + "/" + request.year).Select(x => x.CreatedAt.ToString("dd/MM/yyyy")).Distinct();

            List<StatisticNews> model = new List<StatisticNews>();

            for (var i = 1; i <= 31; i++)
            {
                model.Add(new StatisticNews
                {
                    date = i == 1 ? "Ng " + fromdate.ToString("dd/MM/yyyy") : "Ng " + fromdate.ToString("dd"),
                    countpatient = data.Data.Items.DistinctBy(x => x.Patient.Id).Count(x => x.CreatedAt.ToString("dd/MM/yyyy") == fromdate.ToString("dd/MM/yyyy")),
                    count = data.Data.Items.Count(x => x.CreatedAt.ToString("dd/MM/yyyy") == fromdate.ToString("dd/MM/yyyy")),
                });
                fromdate = fromdate.AddDays(1);
            }
            return model.OrderBy(x => x.date).ToList();
        }
        public async Task<ApiResult<bool>> Update(AppointmentUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/appointment/", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> CanceledAppointment(AppointmentCancelRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/appointment/cancel-appointment", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> AddRate(RateCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/rate/", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }
    }
}
