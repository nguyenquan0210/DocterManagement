using DoctorManagement.Application.Catalog.ScheduleDetailt;
using DoctorManagement.ViewModels.Catalog.ScheduleDetailt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleDetailtController : ControllerBase
    {
        private readonly IScheduleDetailtService _scheduleDetailtService;
        public ScheduleDetailtController(IScheduleDetailtService scheduleDetailtService)
        {
            _scheduleDetailtService = scheduleDetailtService;
        }
        /// <summary>
        /// Tạo mới chi tiết lịch khám
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ScheduleDetailtCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _scheduleDetailtService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa chi tiết lịch khám
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
            var result = await _scheduleDetailtService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật chi tiết lịch khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ScheduleDetailtUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _scheduleDetailtService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang chi tiết lịch khám
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetScheduleDetailtPagingRequest request)
        {
            var user = await _scheduleDetailtService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy chi tiết lịch khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _scheduleDetailtService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chi tiết lịch khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _scheduleDetailtService.GetAll();
            return Ok(result);
        }
    }
}
