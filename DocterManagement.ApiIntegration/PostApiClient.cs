using DoctorManagement.ViewModels.Catalog.Post;
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
    public class PostApiClient : BaseApiClient, IPostApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
                : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<bool>> Create(PostCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ImageFile != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImageFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImageFile.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imageFile", request.ImageFile.FileName);
            }


            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Content.ToString()), "content");
            requestContent.Add(new StringContent(request.TopicId.ToString()), "topicId");
            requestContent.Add(new StringContent(request.DoctorId.ToString()), "doctorId");

            var response = await client.PostAsync($"/api/post", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<int> Delete(Guid Id)
        {
            return await Delete($"/api/post/" + Id);
        }
        public async Task<int> DeleteImg(Guid Id)
        {
            return await Delete($"/api/post/images/" + Id);
        }
        public async Task<int> DeleteAllImg(Guid Id)
        {
            return await Delete($"/api/post/{Id}/delete-all-images");
        }
        public async Task<ApiResult<PostVm>> GetById(Guid Id)
        {
            return await GetAsync<PostVm>($"/api/post/{Id}");
        }

        public async Task<ApiResult<List<PostVm>>> GetAll()
        {
            var data = await GetListAsync<PostVm>($"/api/post/all");
            return data;
        }

        public async Task<ApiResult<PagedResult<PostVm>>> GetAllPaging(GetPostPagingRequest request)
        {
            return await GetAsync<PagedResult<PostVm>>(
               $"/api/post/paging?pageIndex={request.PageIndex}" +
               $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}" +
               $"&usename={request.Usename}");
        }

        public async Task<ApiResult<bool>> Update(PostUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ImageFile != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImageFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImageFile.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imageFile", request.ImageFile.FileName);
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Content.ToString()), "content");
            requestContent.Add(new StringContent(request.TopicId.ToString()), "topicId");
            requestContent.Add(new StringContent(request.DoctorId.ToString()), "doctorId");
            requestContent.Add(new StringContent(request.Status.ToString()), "status");

            var response = await client.PutAsync($"/api/post", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);

        }

        public async Task<ApiResult<string>> AddImage(ImageCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.File != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.File.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.File.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "file", request.File.FileName);
            }
            var response = await client.PostAsync($"/api/storage/post", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(result);
        }
    }
}
