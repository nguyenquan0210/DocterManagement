using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
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
    public class DoctorApiClient : BaseApiClient, IDoctorApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DoctorApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<List<DoctorVm>>> GetTopFavouriteDoctors()
        {
            var data = await GetListAsync<DoctorVm>($"/api/client/get-top-favourite-doctors");
         
            return data;
        }
        public async Task<ApiResult<DoctorVm>> GetById(Guid Id)
        {
            return await GetAsync<DoctorVm>($"/api/client/get-doctors-detailt/{Id}");
        }
        public async Task<ApiResult<PatientVm>> GetByPatientId(Guid Id)
        {
            return await GetAsync<PatientVm>($"/api/client/get-patient-detailt/{Id}");
        }
        public async Task<ApiResult<List<PatientVm>>> GetPatientProfile(string username)
        {
           return  await GetListAsync<PatientVm>($"/api/client/get-patient-profile/{username}");
        }
        public async Task<ApiResult<List<UserVm>>> GetAllUser(string? role)
        {
            return await GetListAsync<UserVm>($"/api/client/get-all-user?role="+role);
        }

        public async Task<ApiResult<bool>> UpdateInfo(UpdatePatientInfoRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/client/update-patient-info", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<Guid>> AddInfo(AddPatientInfoRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/client/add-patient-info", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<Guid>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<Guid>>(result);
        }
    }
}
