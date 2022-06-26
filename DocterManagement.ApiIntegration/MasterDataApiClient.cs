using DoctorManagement.ViewModels.Catalog.MasterData;
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
    public class MasterDataApiClient : BaseApiClient, IMasterDataApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MasterDataApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<bool>> CreateEthnic(EthnicCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/masterData/create-ethnic", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> CreateMainMenu(MainMenuCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.Image != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Image.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Image.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "image", request.Image.FileName);
            }

            requestContent.Add(new StringContent(request.SortOrder.ToString()), "sortOrder");
            requestContent.Add(new StringContent(request.Name.ToString()), "name");
            requestContent.Add(new StringContent(request.Action.ToString()), "action");
            requestContent.Add(new StringContent(request.Controller.ToString()), "controller");
            requestContent.Add(new StringContent(request.ParentId.ToString()), "parentId");
            requestContent.Add(new StringContent(request.Type.ToString()), "type");
            if (request.Title != null)
                requestContent.Add(new StringContent(request.Title.ToString()), "title");
            if (request.Description != null)
                requestContent.Add(new StringContent(request.Description.ToString()), "description");

            var response = await client.PostAsync($"/api/masterData/create-mainmenu", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<int> DeleteEthnic(Guid Id)
        {
            return await Delete($"/api/masterData/delete-ethnic/{Id}");
        }

        public async Task<int> DeleteMainMenu(Guid Id)
        {
            return await Delete($"/api/masterData/delete-mainmenu/{Id}");
        }

        public async Task<ApiResult<List<EthnicsVm>>> GetAllEthnic()
        {
            return  await GetListAsync<EthnicsVm>($"/api/masterData/get-all-ethnic");
        }

        public async Task<ApiResult<List<MainMenuVm>>> GetAllMainMenu()
        {
            return await GetListAsync<MainMenuVm>($"/api/masterData/get-all-mainmenu");

        }

        public async Task<ApiResult<PagedResult<EthnicsVm>>> GetAllPagingEthnic(GetEthnicPagingRequest request)
        {
            return await GetAsync<PagedResult<EthnicsVm>>(
               $"/api/masterData/get-paging-ethnics?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}");
        }

        public async Task<ApiResult<PagedResult<MainMenuVm>>> GetAllPagingMainMenu(GetMainMenuPagingRequest request)
        {
            return await GetAsync<PagedResult<MainMenuVm>>(
              $"/api/masterData/get-paging-mainmenu?pageIndex={request.PageIndex}" +
              $"&pageSize={request.PageSize}" +
              $"&keyword={request.Keyword}" +
              $"&type={request.Type}");
        }

        public async Task<ApiResult<InformationVm>> GetById()
        {
            return await GetAsync<InformationVm>($"/api/masterData/get-by-information");
        }

        public async Task<ApiResult<EthnicsVm>> GetByIdEthnic(Guid Id)
        {
            return await GetAsync<EthnicsVm>($"/api/masterData/get-by-ethnicid/{Id}");
        }

        public async Task<ApiResult<MainMenuVm>> GetByIdMainMenu(Guid Id)
        {
            return await GetAsync<MainMenuVm>($"/api/masterData/get-by-mainmenuid/{Id}");
        }

        public async Task<ApiResult<bool>> Update(InformationUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.Image != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Image.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Image.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "image", request.Image.FileName);
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.TimeWorking.ToString()), "timeWorking");
            requestContent.Add(new StringContent(request.Hotline.ToString()), "hotline");
            requestContent.Add(new StringContent(request.Company.ToString()), "company");
            requestContent.Add(new StringContent(request.FullAddress.ToString()), "fullAddress");
            requestContent.Add(new StringContent(request.Email.ToString()), "email");
            requestContent.Add(new StringContent(request.AccountBankName.ToString()), "accountBankName");
            requestContent.Add(new StringContent(request.AccountBank.ToString()), "accountBank");
            requestContent.Add(new StringContent(request.Content.ToString()), "content");
            requestContent.Add(new StringContent(request.ServiceFee.ToString()), "serviceFee");

            var response = await client.PutAsync($"/api/masterData", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);

        }

        public async Task<ApiResult<bool>> UpdateEthnic(EthnicUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/masterData/update-ethnic", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> UpdateMainMenu(MainMenuUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.Image != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Image.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Image.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "image", request.Image.FileName);
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.SortOrder.ToString()), "sortOrder");
            requestContent.Add(new StringContent(request.Name.ToString()), "name");
            requestContent.Add(new StringContent(request.Action.ToString()), "action");
            requestContent.Add(new StringContent(request.Controller.ToString()), "controller");
            requestContent.Add(new StringContent(request.ParentId.ToString()), "parentId");
            requestContent.Add(new StringContent(request.Type.ToString()), "type");
            requestContent.Add(new StringContent(request.IsDeleted.ToString()), "isDeleted");
            if (request.Title != null)
                requestContent.Add(new StringContent(request.Title.ToString()), "title");
            if (request.Description != null)
                requestContent.Add(new StringContent(request.Description.ToString()), "description");
            var response = await client.PutAsync($"/api/masterData/update-mainmenu", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
    }
}
