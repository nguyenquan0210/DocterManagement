using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.ActiveUsers;
using DoctorManagement.ViewModels.System.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.Catalog.Clinic;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;

namespace DoctorManagement.ApiIntegration
{
    public class UserApiClient : BaseApiClient, IUserApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<bool>> AddUserRole(RequestRoleUser requestRoleUser)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(requestRoleUser);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/users/requestroleuser", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/users/authenticate", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }
        public async Task<ApiResult<string>> CheckPhone(RegisterEnterPhoneRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/users/check-phone", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }
        public async Task<int> IsBooking(Guid Id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/users/doctor-isbooking/{Id}");
            var result = JsonConvert.DeserializeObject<ApiSuccessResult<int>>(await response.Content.ReadAsStringAsync());
            return result.Data;
        }
        public async Task<int> Delete(Guid Id)
        {
            return await Delete($"/api/users/{Id}" );
        }
        public async Task<int> DeleteImg(Guid Id)
        {
            return await Delete($"/api/users/doctor-delete-gallery/{Id}");
        }
        public async Task<int> DeleteAllImg(Guid Id)
        {
            return await Delete($"/api/users/{Id}/doctor-delete-all-gallery");
        }
        public async Task<ApiResult<bool>> UpdateStatus(Guid id, UserUpdateStatusRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/updatestatus{id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ApiResult<UserVm>> GetById(Guid Id)
        {
            return await GetAsync<UserVm>($"/api/users/{Id}");
        }

        public async Task<ApiResult<UserVm>> GetByUserName(string username)
        {
            return await GetAsync<UserVm>($"/api/users/get-by-username/{username}");
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/users/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&rolename={request.RoleName}");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserVm>>>(body);
            return users;
        }

        public async Task<ApiResult<bool>> RegisterUser(ManageRegisterRequest registerRequest)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();


            /*if (registerRequest.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(registerRequest.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)registerRequest.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", registerRequest.ThumbnailImage.FileName);
            }*/

            //requestContent.Add(new StringContent(registerRequest.UserName.ToString()), "userName");
            //requestContent.Add(new StringContent(registerRequest.Password.ToString()), "password");
            requestContent.Add(new StringContent(registerRequest.SpecialityId.ToString()), "specialityId");
            requestContent.Add(new StringContent(registerRequest.ClinicId.ToString()), "clinicId");
            //requestContent.Add(new StringContent(registerRequest.Name.ToString()), "name");
            requestContent.Add(new StringContent(registerRequest.Gender.ToString()), "gender");
            requestContent.Add(new StringContent(registerRequest.Dob.ToString()), "dob");
            requestContent.Add(new StringContent(registerRequest.Address.ToString()), "address");
            requestContent.Add(new StringContent(registerRequest.Email.ToString()), "email");
            requestContent.Add(new StringContent(registerRequest.PhoneNumber.ToString()), "phoneNumber");

            var response = await client.PostAsync($"/api/users/register-doctor", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<bool>> UpdateDoctor(Guid id, UserUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.Galleries != null)
            {
                byte[] data;
                foreach (var gallery in request.Galleries)
                {
                    using (var br = new BinaryReader(gallery.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)gallery.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "galleries", gallery.FileName);
                }
            }
            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }
            foreach (var speciality in request.Specialities)
            {
                requestContent.Add(new StringContent(speciality.ToString()), "specialities");
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.Status.ToString()), "status");
            requestContent.Add(new StringContent(request.MapUrl.ToString()), "mapUrl");
            requestContent.Add(new StringContent(request.Slug.ToString()), "slug");
            requestContent.Add(new StringContent(request.Prefix.ToString()), "prefix");
            requestContent.Add(new StringContent(request.IsPrimary.ToString()), "isPrimary");
            requestContent.Add(new StringContent(request.Booking.ToString()), "booking");
            requestContent.Add(new StringContent(request.SubDistrictId.ToString()), "subDistrictId");
            requestContent.Add(new StringContent(request.DistrictId.ToString()), "districtId");
            requestContent.Add(new StringContent(request.ProvinceId.ToString()), "provinceId");
            requestContent.Add(new StringContent(request.FirstName.ToString()), "firstName");
            requestContent.Add(new StringContent(request.LastName.ToString()), "lastName");
            requestContent.Add(new StringContent(request.Address.ToString()), "address");
            //requestContent.Add(new StringContent(request.Email.ToString()), "email");
            //requestContent.Add(new StringContent(request.PhoneNumber.ToString()), "phoneNumber");
            requestContent.Add(new StringContent(request.ClinicId.ToString()), "clinicId");
            requestContent.Add(new StringContent(request.Gender.ToString()), "gender");
            requestContent.Add(new StringContent(request.Dob.ToString()), "dob");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Educations.ToString()), "educations");
            requestContent.Add(new StringContent(request.Services.ToString()), "services");
            requestContent.Add(new StringContent(request.Note.ToString()), "note");
            requestContent.Add(new StringContent(request.Prizes.ToString()), "prizes");
            requestContent.Add(new StringContent(request.TimeWorking.ToString()), "timeWorking");


            var response = await client.PutAsync($"/api/users/update-doctor/{id}", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> DoctorUpdateProfile(DoctorUpdateProfile request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/doctor/update-doctor-profile", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
        public async Task<ApiResult<bool>> DoctorUpdateRequest(Guid id, DoctorUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.Galleries != null)
            {
                byte[] data;
                foreach (var gallery in request.Galleries)
                {
                    using (var br = new BinaryReader(gallery.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)gallery.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "galleries", gallery.FileName);
                }
            }
            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }
            foreach (var speciality in request.Specialities)
            {
                requestContent.Add(new StringContent(speciality.ToString()), "specialities");
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.MapUrl.ToString()), "mapUrl");
            requestContent.Add(new StringContent(request.Slug.ToString()), "slug");
            requestContent.Add(new StringContent(request.Prefix.ToString()), "prefix");
            requestContent.Add(new StringContent(request.Booking.ToString()), "booking");
            requestContent.Add(new StringContent(request.SubDistrictId.ToString()), "subDistrictId");
            requestContent.Add(new StringContent(request.DistrictId.ToString()), "districtId");
            requestContent.Add(new StringContent(request.ProvinceId.ToString()), "provinceId");
            requestContent.Add(new StringContent(request.FirstName.ToString()), "firstName");
            requestContent.Add(new StringContent(request.LastName.ToString()), "lastName");
            requestContent.Add(new StringContent(request.Address.ToString()), "address");
            requestContent.Add(new StringContent(request.ClinicId.ToString()), "clinicId");
            requestContent.Add(new StringContent(request.Gender.ToString()), "gender");
            requestContent.Add(new StringContent(request.Dob.ToString()), "dob");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Educations.ToString()), "educations");
            requestContent.Add(new StringContent(request.Services.ToString()), "services");
            requestContent.Add(new StringContent(request.Note.ToString()), "note");
            requestContent.Add(new StringContent(request.Prizes.ToString()), "prizes");
            requestContent.Add(new StringContent(request.TimeWorking.ToString()), "timeWorking");


            var response = await client.PutAsync($"/api/users/doctor/update-doctor-request/{id}", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
        public async Task<ApiResult<bool>> PublicRegisterUser(PublicRegisterRequest registerRequest)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(registerRequest);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/users/", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/changepass", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<List<GetMonth>> GetActiveUserDay(string month, string year)
        {
            var response = await GetListAsync<ActiveUserVm>($"/api/users/activeusers");

            List<GetMonth> counts = new List<GetMonth>();
            var date = response.Data.Where(x => x.DateActive.ToString("MM/yyyy") == month.ToString() + "/" + year.ToString()).Select(x => x.DateActive.ToString("dd/MM/yyyy")).Distinct();

            foreach (var item in date)
            {
                counts.Add(new GetMonth { date = item, count = response.Data.Count(x => x.DateActive.ToString("dd/MM/yyyy") == item) });

            }
            return counts;
        }
        public async Task<List<RoleVm>> GetAllRole()
        {
            var data = await GetListAsync<RoleVm>($"/api/users/get-all-role");
            return data.Data;
        }

        public async Task<List<UserVm>> GetNewUsers()
        {
            var data = await GetListAsync<UserVm>($"/api/users/newuser");
            return data.Data;
        }

        public async Task<List<ActiveUserVm>> GetActiveUser()
        {
            var data = await GetListAsync<ActiveUserVm>($"/api/users/activeusers");
            return data.Data;
        }

        public Task<List<StatisticNews>> GetUserStatiticMonth(string month, string year)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatisticNews>> GetUserStatiticDay(string day, string month, string year)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatisticNews>> GetUserStatiticYear(string year)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SelectListItem>> GetAllClinic(Guid? clinicId)
        {
            var data = await GetListAsync<ClinicVm>($"/api/clinic/get-all-clinic");
            var select = data.Data.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = clinicId.HasValue && clinicId.Value == x.Id
            });
            return select.ToList();
        }
        public async Task<List<SelectListItem>> GetAllSpeciality(Guid? specialityId)
        {
            var data = await GetListAsync<SpecialityVm>($"/api/speciality/get-all-speciality");
            var select = data.Data.Select(x => new SelectListItem()
            {
                Text = x.Title,
                Value = x.Id.ToString(),
                Selected = specialityId.HasValue && specialityId.Value == x.Id
            });
            return select.ToList();
        }

        public async Task<ApiResult<bool>> UpdateAdmin(UserUpdateAdminRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/update-admin", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        

        public async Task<ApiResult<bool>> UpdatePatient(Guid id, UserUpdatePatientRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            requestContent.Add(new StringContent(request.Status.ToString()), "status");

            requestContent.Add(new StringContent(request.Name.ToString()), "name");
            requestContent.Add(new StringContent(request.Gender.ToString()), "gender");
            requestContent.Add(new StringContent(request.Dob.ToString()), "dob");
            requestContent.Add(new StringContent(request.Address.ToString()), "address");
            requestContent.Add(new StringContent(request.Email.ToString()), "email");
            requestContent.Add(new StringContent(request.PhoneNumber.ToString()), "phoneNumber");

            var response = await client.PutAsync($"/api/users/update-patient/{id}", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> RegisterDocter(ManageRegisterRequest registerRequest)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(registerRequest);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/users/register-doctor", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
        public async Task<ApiResult<bool>> RegisterPatient(RegisterEnterProfileRequest registerRequest)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(registerRequest);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/users/register-patient", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
    public async Task<List<SelectListItem>> GetAllEthnicGroup()
        {
            var data = await GetListAsync<EthnicVm>($"/api/users/get-all-ethnicgroup");
            var select = data.Data.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return select.ToList();
        }
    }
}
