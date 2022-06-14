using DoctorManagement.Application.System.Users;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly IActiveUserService _activeUserService;
        private readonly ITwilioRestClient _client;
        private readonly IConfiguration _configuration;
        public UsersController(IUserService userService, ITwilioRestClient client,
            IConfiguration configuration)//IActiveUserService activeUserService
        {
            _userService = userService;
            //_activeUserService = activeUserService;
            _client = client;
            _configuration = configuration;
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
        /// Kiểm tra số điện thoại gữi mã otp
        /// </summary>
        /// 
        [AllowAnonymous]
        [HttpPost("check-phone")]
        public async Task<IActionResult> CheckPhone(RegisterEnterPhoneRequest request)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CheckPhone(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
           /* request.PhoneNumber = "+84" + request.PhoneNumber.Substring(1, 9);
            var messageOtp = result.Data + " la ma xac thuc của quy khach tren ung dung DatKham. Ma co hieu luc trong 2 phut.";
            
            var message = MessageResource.Create(
               to: new PhoneNumber(request.PhoneNumber),
               from: new PhoneNumber(_configuration["Twilio:PhoneNumber"]),
               body: messageOtp,
               client: _client); // pass in the custom client*/

            return Ok(result);
        }
        /// <summary>
        /// Đăng ký tài khoản bác sĩ từ người quản trị
        /// </summary>
        /// 
        [HttpPost("register-doctor")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> ManageRegister(ManageRegisterRequest request)
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
        /// Đăng ký tài khoản bệnh nhân trang người dùng
        /// </summary>
        /// 
        [HttpPost("register-patient")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPatient(RegisterEnterProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterPatient(request);
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
        public async Task<IActionResult> UpdateDoctor(Guid id,UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateDoctor(id, request);
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
        [HttpPut("doctor/update-doctor-profile")]
        public async Task<IActionResult> DoctorUpdateProfile(DoctorUpdateProfile request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.DoctorUpdateProfile(request);
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
        [HttpPut("doctor/update-doctor-request/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> DoctorUpdateRequest(Guid id, [FromForm] DoctorUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.DoctorUpdateRequest(id, request);
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
        /// Quên mật khẩu
        /// </summary>
        /// 
        [HttpPut("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ForgotPassword(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// cài lại mật khẩu
        /// </summary>
        /// 
        [HttpPut("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ResetPassword(request);
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
        [HttpGet("get-by-username/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByUserName(string username)
        {
            var user = await _userService.GetByUserName(username);
            return Ok(user);
        }
        /// <summary>
        /// Cập nhật cho phép đặt khám
        /// </summary>
        /// 
        [HttpGet("doctor-isbooking/{Id}")]
        public async Task<IActionResult> IsBooking(Guid Id)
        {
            var result = await _userService.IsBooking(Id);
            return Ok(result);
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
        /// <summary>
        /// Xóa ảnh bộ siêu tập
        /// </summary>
        /// 
        [HttpDelete("doctor-delete-gallery/{Id}")]
        public async Task<IActionResult> DeleteImg(Guid Id)
        {
            var result = await _userService.DeleteImg(Id);
            return Ok(result);
        }
        /// <summary>
        /// Xóa ảnh bộ siêu tập
        /// </summary>
        /// 
        [HttpDelete("{Id}/doctor-delete-all-gallery")]
        public async Task<IActionResult> DeleteAllImg(Guid Id)
        {
            var result = await _userService.DeleteAllImg(Id);
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
        /// <summary>
        /// Lấy tất cả danh sách tỉnh/thành phố
        /// </summary>
        /// 
        [HttpGet("get-all-ethnicgroup")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<List<EthnicVm>>>> GetAllEthnicGroup()
        {
            var result = await _userService.GetAllEthnicGroup();
            return Ok(result);
        }
    }
}
