using DoctorManagement.Application.Catalog.Ward;
using DoctorManagement.ViewModels.Catalog.Ward;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardController : ControllerBase
    {
        private readonly IWardService _wardService;
        public WardController(IWardService WardService)
        {
            _wardService = WardService;
        }
        /// <summary>
        /// Tạo mới phường/xã
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<WardVm>>> Create([FromBody] WardCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _wardService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
        /// <summary>
        /// Xóa phường/xã
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
            var result = await _wardService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật phường/xã
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<WardVm>>> Update([FromBody] WardUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _wardService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang phường/xã
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<WardVm>>>> GetAllPaging([FromQuery] GetWardPagingRequest request)
        {
            var result = await _wardService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy phường/xã theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<WardVm>>> GetById(Guid Id)
        {
            var result = await _wardService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find ward");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách phường/xã
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<WardVm>>>> GetAll()
        {
            var result = await _wardService.GetAll();
            return Ok(result);
        }
    }
}
