using DoctorManagement.Application.System.Doctor;
using DoctorManagement.Application.System.Users;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
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
            return Ok(user);
        }
    }
}
