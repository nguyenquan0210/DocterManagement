using DoctorManagement.Application.System.Doctor;
using DoctorManagement.Application.System.Users;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDoctorService _doctorService;
        private readonly IConfiguration _configuration;
        public ClientController(IUserService userService, IDoctorService doctorService,
            IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _doctorService = doctorService;
        }
        /// <summary>
        /// Lấy danh sách phân trang tài khoản
        /// </summary>
        /// 
        [HttpGet("get-top-favourite-doctors")]
        public async Task<ActionResult<ApiResult<List<DoctorVm>>>> GetTopFavouriteDoctors()
        {
            var user = await _doctorService.GetTopFavouriteDoctors();
            return Ok(user);
        }
        /// <summary>
        /// Lấy thông tin bác sĩ
        /// </summary>
        /// 
        [HttpGet("get-doctors-detailt/{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var user = await _doctorService.GetById(Id);
            if(!user.IsSuccessed) return BadRequest(user);
            return Ok(user);
        }
        /// <summary>
        /// Lấy thông tin bác sĩ
        /// </summary>
        /// 
        [HttpGet("get-patient-detailt/{Id}")]
        public async Task<IActionResult> GetByPatientId(Guid Id)
        {
            var user = await _doctorService.GetByPatientId(Id);
            if (!user.IsSuccessed) return BadRequest(user);
            return Ok(user);
        }
        /// <summary>
        /// Lấy danh sách phân trang tài khoản
        /// </summary>
        /// 
        [HttpGet("get-patient-profile/{userName}")]
        public async Task<ActionResult<ApiResult<List<DoctorVm>>>> GetPatientProfile(string userName)
        {
            var user = await _doctorService.GetPatientProfile(userName);
            return Ok(user);
        }
        /// <summary>
        /// Lấy tất cả danh sách người dùng theo vai trò
        /// </summary>
        /// 
        [HttpGet("get-all-user")]
        public async Task<ActionResult<ApiResult<List<UserVm>>>> GetAllUser(string? role)
        {
            var user = await _doctorService.GetAllUser(role);
            return Ok(user);
        }
        /// <summary>
        /// Cập nhật hồ sơ bệnh nhân
        /// </summary>
        /// 
        [HttpPut("update-patient-info")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateInfo(UpdatePatientInfoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _doctorService.UpdateInfo(request);
            if(!result.IsSuccessed) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật hồ sơ bệnh nhân
        /// </summary>
        /// 
        [HttpPost("add-patient-info")]
        public async Task<ActionResult<ApiResult<bool>>> AddInfo(AddPatientInfoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _doctorService.AddInfo(request);
            if (!result.IsSuccessed) return BadRequest(result);
            return Ok(result);
        }
    }
}
