using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.ActiveUsers;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
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

        Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request);

        Task<List<UserVm>> GetNewUsers();

        Task<ApiResult<bool>> RegisterUser(ManageRegisterRequest registerRequest);

        Task<ApiResult<bool>> PublicRegisterUser(PublicRegisterRequest registerRequest);

        Task<ApiResult<bool>> AddUserRole(RequestRoleUser request);

        Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request);

        Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<UserVm>> GetByUserName(string username);

        Task<int> Delete(Guid Id);

        Task<ApiResult<bool>> UpdateStatus(Guid id, UserUpdateStatusRequest request);

        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);

        Task<List<GetMonth>> GetActiveUserDay(string month, string year);

        Task<List<RoleVm>> GetAllRole();

        Task<List<ActiveUserVm>> GetActiveUser();

        Task<List<StatisticNews>> GetUserStatiticMonth(string month, string year);
        Task<List<StatisticNews>> GetUserStatiticDay(string day, string month, string year);
        Task<List<StatisticNews>> GetUserStatiticYear(string year);
    }
}
