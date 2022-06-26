using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.Users
{
    public interface IUserService 
    {
        Task<ApiResult<string>> Authencate(LoginRequest request);
        Task<ApiResult<string>> CheckPhone(RegisterEnterPhoneRequest request);
        Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request);

        Task<ApiResult<bool>> ForgotPassword(ForgotPasswordRequest request);
        Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest model);
        Task<ApiResult<bool>> Register(PublicRegisterRequest request);

        Task<ApiResult<bool>> ManageRegister(ManageRegisterRequest request);
        Task<ApiResult<bool>> RegisterPatient(RegisterEnterProfileRequest request);

        Task<ApiResult<bool>> AddRoleUser(RequestRoleUser request);

        Task<ApiResult<bool>> UpdateDoctor(Guid id, UserUpdateRequest request);
        Task<ApiResult<bool>> DoctorUpdateRequest(Guid id, DoctorUpdateRequest request);
        Task<ApiResult<bool>> DoctorUpdateProfile(DoctorUpdateProfile request);

        Task<ApiResult<bool>> UpdateAdmin(UserUpdateAdminRequest request);
        Task<ApiResult<bool>> UpdatePatient(Guid id, UserUpdatePatientRequest request);

        ApiResult<PagedResult<UserVm>> GetUsersPaging(GetUserPagingRequest request);

        Task<ApiResult<PagedResult<UserVm>>> GetUsersAllPaging(GetUserPagingRequest request);
        Task<ApiResult<PagedResult<UserVm>>> GetDoctorAllPaging(GetUserPagingRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<UserVm>> GetByUserName(string username);
        Task<ApiResult<int>> IsBooking(Guid Id);
        Task<ApiResult<int>> Delete(Guid id);
        Task<ApiResult<int>> DeleteImg(Guid Id);
        Task<ApiResult<int>> DeleteAllImg(Guid Id);

        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);

        Task<ApiResult<List<RoleVm>>> GetAllRole();

        Task<ApiResult<List<RoleVm>>> GetAllRoleData();

        List<UserVm> GetNewUser();

        Task<ApiResult<List<EthnicVm>>> GetAllEthnicGroup();
    }
}
