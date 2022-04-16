using DoctorManagement.Application.System.Users;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly IActiveUserService _activeUserService;

        public UsersController(IUserService userService )//IActiveUserService activeUserService
        {
            _userService = userService;
            //_activeUserService = activeUserService;
        }
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// 
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(result.Data))
            {
                return BadRequest(result);
            }
            /*if (request.Check == false)
            {
                var listActive = _activeUserService.ListActiveUser().Result.Where(x => x.DateActive.ToShortDateString() == DateTime.Now.ToShortDateString());

                var user = await _userService.GetByUserName(request.UserName);
                var activeUser = listActive.FirstOrDefault(x => x.UserId == user.Data.Id);
                if (activeUser != null)
                    await _activeUserService.UpdateActiveUser(activeUser.Id);
                else
                    await _activeUserService.AddActiveUser(user.Data.Id);
            }*/

            return Ok(result);
        }
        /// <summary>
        /// Đăng ký tài khoản bác sĩ từ người quản trị
        /// </summary>
        /// 
        [HttpPost("register-doctor")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ManageRegister([FromForm] ManageRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ManageRegister(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật tài khoản
        /// </summary>
        /// 
        [HttpPut("update-doctor/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromForm] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update-admin")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UserUpdateAdminRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateAdmin(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update-patient/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromForm] UserUpdatePatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdatePatient(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// 
        [HttpPut("changepass")]
        public async Task<IActionResult> ChangePass([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePassword(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Thêm vai trong mới cho tài khoản
        /// </summary>
        /// 
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang tài khoản
        /// </summary>
        /// 
        [HttpGet("paging")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            /*if (request.RoleName != null)
            {
                if (request.RoleName.ToUpper() != "ALL")
                {
                    var userinrole = _userService.GetUsersPaging(request);
                    return Ok(userinrole);
                }
            }*/
            var user = await _userService.GetUsersAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy tài khoản theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var user = await _userService.GetById(Id);
            return Ok(user);
        }
        /// <summary>
        /// Lấy tài khoản theo tên tài khoản
        /// </summary>
        /// 
        [HttpGet("get-request{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByUserName(string username)
        {
            var user = await _userService.GetByUserName(username);
            return Ok(user);
        }
        /// <summary>
        /// Xóa tài khoản
        /// </summary>
        /// 
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _userService.Delete(Id);
            return Ok(result);
        }
        /*[HttpGet("activeusers")]
        public async Task<IActionResult> GetActiveUser()
        {
            var activeUsers = await _activeUserService.ListActiveUser();
            return Ok(activeUsers);
        }*/
        /// <summary>
        /// Lấy tất cả danh sách vai trò người dùng
        /// </summary>
        /// 
        [HttpGet("get-all-role")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<List<RoleVm>>>> GetAllRole()
        {
            var roles = await _userService.GetAllRole();
            return Ok(roles);
        }
        /// <summary>
        /// Lấy tất cả danh sách vai trò người dùng trả về data
        /// </summary>
        /// 
        [HttpGet("get-all-role-data")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRoleData()
        {
            var roles = await _userService.GetAllRoleData();
            return Ok(roles);
        }
        /// <summary>
        /// Lấy tất cả danh sách tài khoản theo vai trò người dùng bệnh nhân
        /// </summary>
        /// 
        [HttpGet("newuser")]
        public IActionResult GetNewUser()
        {
            var activeUsers = _userService.GetNewUser();
            return Ok(activeUsers);
        }
    }
}
