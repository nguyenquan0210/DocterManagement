using DoctorManagement.Application.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.Appointment;
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
        public async Task<IActionResult> Create([FromBody] AppointmentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _appointmentService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa đặt khám
        /// </summary>
        /// 
        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid Id)
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
        public async Task<IActionResult> Update([FromBody] AppointmentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _appointmentService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }

        /// <summary>
        /// Lấy danh sách đặt khám phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetAppointmentPagingRequest request)
        {
            var user = await _appointmentService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy dự liệu đặt khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _appointmentService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách đặt khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appointmentService.GetAll();
            return Ok(result);
        }
    }
}
