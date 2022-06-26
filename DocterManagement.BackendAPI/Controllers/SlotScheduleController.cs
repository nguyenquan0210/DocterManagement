using DoctorManagement.Application.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.Catalog.SlotSchedule;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SlotScheduleController : ControllerBase
    {
        private readonly ISlotScheduleService _slotScheduleService;
        public SlotScheduleController(ISlotScheduleService slotScheduleService)
        {
            _slotScheduleService = slotScheduleService;
        }
        /// <summary>
        /// Tạo mới chi tiết lịch khám
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] SlotScheduleCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _slotScheduleService.Create(request);
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
            var result = await _slotScheduleService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật chi tiết lịch khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] SlotScheduleUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _slotScheduleService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang chi tiết lịch khám
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<SlotScheduleVm>>>> GetAllPaging([FromQuery] GetSlotSchedulePagingRequest request)
        {
            var result = await _slotScheduleService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy chi tiết lịch khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<SlotScheduleVm>>> GetById(Guid Id)
        {
            var result = await _slotScheduleService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chi tiết lịch khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<SlotScheduleVm>>>> GetAll()
        {
            var result = await _slotScheduleService.GetAll();
            return Ok(result);
        }
    }
}
