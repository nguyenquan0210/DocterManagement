using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class LocationApiClient : BaseApiClient, ILocationApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocationApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<bool>> Create(LocationCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/location/create-location", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<int> Delete(Guid Id)
        {
            return await Delete($"/api/location/delete" + Id);
        }

        public async Task<List<SelectListItem>> GetAllDistrict(Guid? provinceId)
        {
            var data = await GetListAsync<LocationVm>($"/api/location/get-all-district");
            var select = data.Data.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = provinceId.HasValue && provinceId.Value == x.Id
            });
            return select.ToList();
        }

        public async Task<List<SelectListItem>> GetAllProvince(Guid? provinceId)
        {
            var data = await GetListAsync<LocationVm>($"/api/location/get-all-province");
            var select = data.Data.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = provinceId.HasValue && provinceId.Value == x.Id
            });
            return select.ToList();
        }

        public async Task<List<SelectListItem>> GetAllSubDistrict(Guid? provinceId)
        {
            var data = await GetListAsync<LocationVm>($"/api/location/get-all-subDistrict");
            var select = data.Data.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = provinceId.HasValue && provinceId.Value == x.Id
            });
            return select.ToList();
        }

        public async Task<ApiResult<LocationVm>> GetById(Guid Id)
        {
            return await GetAsync<LocationVm>($"/api/location/get-by-id/{Id}");
        }

        public async Task<ApiResult<PagedResult<LocationVm>>> GetLocationPagings(GetLocationPagingRequest request)
        {
            return await GetAsync<PagedResult<LocationVm>>(
               $"/api/location/get-paging-location?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}" +
               $"&type={request.Type}");
        }

        public async Task<ApiResult<bool>> Update(LocationUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/location/update-location", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }
    }
}
