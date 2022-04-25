using DoctorManagement.Application.Catalog.Appointment;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Tạo mới đặt khám
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] AppointmentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _appointmentService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
        /// <summary>
        /// Xóa đặt khám
        /// </summary>
        /// 
        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> Delete([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _appointmentService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật đặt khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] AppointmentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _appointmentService.Update(request);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách đặt khám phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<AppointmentVm>>>> GetAllPaging([FromQuery] GetAppointmentPagingRequest request)
        {
            var user = await _appointmentService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy dự liệu đặt khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<AppointmentVm>>> GetById(Guid Id)
        {
            var result = await _appointmentService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find appointment");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách đặt khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<AppointmentVm>>>> GetAll()
        {
            var result = await _appointmentService.GetAll();
            return Ok(result);
        }
    }
}
