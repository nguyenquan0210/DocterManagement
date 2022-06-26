using DoctorManagement.Application.Catalog.Schedule;
using DoctorManagement.ViewModels.Catalog.Schedule;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        /// <summary>
        /// Tạo mới lịch khám
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] ScheduleCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _scheduleService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa lịch khám
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
            var result = await _scheduleService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật lịch khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] ScheduleUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _scheduleService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang lịch khám 
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<ScheduleVm>>>> GetAllPaging([FromQuery] GetSchedulePagingRequest request)
        {
            var result = await _scheduleService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy lịch khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<ScheduleVm>>> GetById(Guid Id)
        {
            var result = await _scheduleService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find schedule");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách lịch khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<ScheduleVm>>>> GetAll()
        {
            var result = await _scheduleService.GetAll();
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách lịch khám thuộc bác sĩ
        /// </summary>
        /// 
        [HttpGet("GetScheduleDoctor/{Id}")]
        public async Task<ActionResult<ApiResult<List<DoctorScheduleClientsVm>>>> GetAll(Guid Id)
        {
            var result = await _scheduleService.GetScheduleDoctor(Id);
            return Ok(result);
        }
    }
}
