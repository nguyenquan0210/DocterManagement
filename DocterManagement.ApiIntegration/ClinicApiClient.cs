using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public class ClinicApiClient : BaseApiClient,IClinicApiClient 
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

    public ClinicApiClient(IHttpClientFactory httpClientFactory,
               IHttpContextAccessor httpContextAccessor,
                IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<bool>> Create(ClinicCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.ImgClinics != null)
            {
                byte[] data;
                foreach (var imgClinic in request.ImgClinics)
                {
                    using (var br = new BinaryReader(imgClinic.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)imgClinic.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "imgClinics", imgClinic.FileName);
                }
                
            }

            if (request.ImgLogo != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImgLogo.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImgLogo.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imgLogo", request.ImgLogo.FileName);
            }

          
            requestContent.Add(new StringContent(request.LocationId.ToString()), "locationId");
            requestContent.Add(new StringContent(request.Name.ToString()), "name");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Address.ToString()), "address");

            var response = await client.PostAsync($"/api/clinic", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<int> Delete(Guid Id)
        {
            return await Delete($"/api/clinic/" + Id);
        }
        public async Task<int> DeleteImg(Guid Id)
        {
            return await Delete($"/api/clinic/images/" + Id);
        }
        public async Task<ApiResult<ClinicVm>> GetById(Guid Id)
        {
            return await GetAsync<ClinicVm>($"/api/clinic/{Id}");
        }

        public async Task<ApiResult<List<ClinicVm>>> GetMenu()
        {
            var data = await GetListAsync<ClinicVm>($"/api/clinic/all");
            return data;
        }

        public async Task<ApiResult<PagedResult<ClinicVm>>> GetClinicPagings(GetClinicPagingRequest request)
        {
            return await GetAsync<PagedResult<ClinicVm>>(
               $"/api/clinic/paging?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}");
        }

        public async Task<ApiResult<bool>> Update(ClinicUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.ImgClinics != null)
            {
                byte[] data;
                foreach (var imgClinic in request.ImgClinics)
                {
                    using (var br = new BinaryReader(imgClinic.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)imgClinic.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "imgClinics", imgClinic.FileName);
                }

            }

            if (request.ImgLogo != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImgLogo.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImgLogo.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imgLogo", request.ImgLogo.FileName);
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.LocationId.ToString()), "locationId");
            requestContent.Add(new StringContent(request.Name.ToString()), "name");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Address.ToString()), "address");
            requestContent.Add(new StringContent(request.Status.ToString()), "status");

            var response = await client.PutAsync($"/api/clinic", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);

        }
    }
}
