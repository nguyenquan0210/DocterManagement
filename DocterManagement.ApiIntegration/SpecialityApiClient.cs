using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Http;
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
    public class SpecialityApiClient : BaseApiClient, ISpecialityApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SpecialityApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<bool>> Create(SpecialityCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            
            if (request.Img != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Img.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Img.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "img", request.Img.FileName);
            }
            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            if(request.Description!=null) requestContent.Add(new StringContent(request.Description.ToString()), "description");

            var response = await client.PostAsync($"/api/speciality", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<int> Delete(Guid Id)
        {
            return await Delete($"/api/speciality/" + Id);
        }

        public async Task<ApiResult<SpecialityVm>> GetById(Guid Id)
        {
            return await GetAsync<SpecialityVm>($"/api/speciality/{Id}");
        }

        public async Task<ApiResult<List<SpecialityVm>>> GetAllSpeciality()
        {
            return await GetListAsync<SpecialityVm>($"/api/speciality/get-all-speciality");
        }

        public async Task<ApiResult<PagedResult<SpecialityVm>>> GetSpecialityPagings(GetSpecialityPagingRequest request)
        {
            return await GetAsync<PagedResult<SpecialityVm>>(
               $"/api/speciality/paging?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}");
        }

        public async Task<ApiResult<bool>> Update(SpecialityUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.Img != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Img.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Img.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "img", request.Img.FileName);
            }
            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.IsDeleted.ToString()), "isDeleted");
            requestContent.Add(new StringContent(request.SortOrder.ToString()), "sortOrder");
            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            if(request.Description!=null)requestContent.Add(new StringContent(request.Description.ToString()), "description");

            var response = await client.PutAsync($"/api/speciality", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
    }
}
