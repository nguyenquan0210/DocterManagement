using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.ActiveUsers;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);
        Task<ApiResult<string>> CheckPhone(RegisterEnterPhoneRequest request);
        Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request);

        Task<List<UserVm>> GetNewUsers();

        Task<ApiResult<bool>> RegisterUser(ManageRegisterRequest registerRequest);
        Task<ApiResult<bool>> RegisterDocter(ManageRegisterRequest registerRequest);
        Task<ApiResult<bool>> RegisterPatient(RegisterEnterProfileRequest registerRequest);

        Task<ApiResult<bool>> PublicRegisterUser(PublicRegisterRequest registerRequest);

        Task<ApiResult<bool>> AddUserRole(RequestRoleUser request);

        Task<ApiResult<bool>> UpdateDoctor(Guid id, UserUpdateRequest request);
        Task<ApiResult<bool>> DoctorUpdateRequest(Guid id, DoctorUpdateRequest request);
        Task<ApiResult<bool>> DoctorUpdateProfile(DoctorUpdateProfile request);
        Task<ApiResult<bool>> UpdateAdmin(UserUpdateAdminRequest request);
        Task<ApiResult<bool>> UpdatePatient(Guid id, UserUpdatePatientRequest request);

        Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<UserVm>> GetByUserName(string username);

        Task<int> IsBooking(Guid Id);
        Task<int> Delete(Guid Id);
        Task<int> DeleteImg(Guid Id);
        Task<int> DeleteAllImg(Guid Id);
        Task<List<SelectListItem>> GetAllClinic(Guid? clinicId);
        Task<List<SelectListItem>> GetAllSpeciality(Guid? specialityId);

        Task<ApiResult<bool>> UpdateStatus(Guid id, UserUpdateStatusRequest request);

        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);

        Task<List<GetMonth>> GetActiveUserDay(string month, string year);

        Task<List<RoleVm>> GetAllRole();

        Task<List<ActiveUserVm>> GetActiveUser();

        Task<List<StatisticNews>> GetUserStatiticMonth(string month, string year);
        Task<List<StatisticNews>> GetUserStatiticDay(string day, string month, string year);
        Task<List<StatisticNews>> GetUserStatiticYear(string year);

        Task<List<SelectListItem>> GetAllEthnicGroup();
    }
}
