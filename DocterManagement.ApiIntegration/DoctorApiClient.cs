using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
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
    }
}
