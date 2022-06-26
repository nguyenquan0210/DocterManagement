using DoctorManagement.Application.Catalog.Distric;
using DoctorManagement.ViewModels.Catalog.Distric;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistricController : ControllerBase
    {
        private readonly IDistricService _districService;
        public DistricController(IDistricService districService)
        {
            _districService = districService;
        }
        /// <summary>
        /// Tạo mới quận/huyện
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<DistricVm>>> Create([FromBody] DistricCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _districService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa quận/huyện
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
            var result = await _districService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật quận/huyện
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<DistricVm>>> Update([FromBody] DistricUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _districService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang của quận/huyện
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<DistricVm>>>> GetAllPaging([FromQuery] GetDistricPagingRequest request)
        {
            var apiResult = await _districService.GetAllPaging(request);
            return Ok(apiResult);
        }
        /// <summary>
        /// Lấy quận/huyện theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<DistricVm>>> GetById(Guid Id)
        {
            var result = await _districService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find distric");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách quận/huyện
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<DistricVm>>>> GetAll()
        {
            var result = await _districService.GetAll();
            return Ok(result);
        }
    }
}