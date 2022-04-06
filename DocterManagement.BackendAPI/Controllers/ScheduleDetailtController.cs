using DoctorManagement.Application.Catalog.ScheduleDetailt;
using DoctorManagement.ViewModels.Catalog.ScheduleDetailt;
using DoctorManagement.ViewModels.Common;
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
        public async Task<ActionResult<ApiResult<ScheduleDetailtVm>>> Create([FromBody] ScheduleDetailtCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _scheduleDetailtService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
        /// <summary>
        /// Xóa chi tiết lịch khám
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
            var result = await _scheduleDetailtService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật chi tiết lịch khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<ScheduleDetailtVm>>> Update([FromBody] ScheduleDetailtUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _scheduleDetailtService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang chi tiết lịch khám
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<ScheduleDetailtVm>>>> GetAllPaging([FromQuery] GetScheduleDetailtPagingRequest request)
        {
            var result = await _scheduleDetailtService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy chi tiết lịch khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<ScheduleDetailtVm>>> GetById(Guid Id)
        {
            var result = await _scheduleDetailtService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find schedule detailt");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chi tiết lịch khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<ScheduleDetailtVm>>>> GetAll()
        {
            var result = await _scheduleDetailtService.GetAll();
            return Ok(result);
        }
    }
}
