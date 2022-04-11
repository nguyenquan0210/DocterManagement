using DoctorManagement.Application.Catalog.Ward;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }
        /// <summary>
        /// Tạo mới phường/xã
        /// </summary>
        /// 
        [HttpPost("create-location")]
        [Authorize]
        public async Task<ActionResult<ApiResult<LocationVm>>> Create([FromBody] LocationCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _locationService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
        /// <summary>
        /// Xóa phường/xã
        /// </summary>
        /// 
        [HttpDelete("delete/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> Delete([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _locationService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật phường/xã
        /// </summary>
        /// 
        [HttpPut("update-location")]
        [Authorize]
        public async Task<ActionResult<ApiResult<LocationVm>>> Update([FromBody] LocationUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _locationService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang phường/xã
        /// </summary>
        /// 
        [HttpGet("get-paging-location")]
        public async Task<ActionResult<ApiResult<PagedResult<LocationVm>>>> GetAllPaging([FromQuery] GetLocationPagingRequest request)
        {
            var result = await _locationService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy phường/xã theo id
        /// </summary>
        /// 
        [HttpGet("get-by-id/{Id}")]
        public async Task<ActionResult<ApiResult<LocationVm>>> GetById(Guid Id)
        {
            var result = await _locationService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find ward");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách phường/xã
        /// </summary>
        /// 
        [HttpGet("get-all-subDistrict")]
        public async Task<ActionResult<ApiResult<List<LocationVm>>>> GetAllSubDistrict()
        {
            var result = await _locationService.GetAllSubDistrict();
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách quận/huyện
        /// </summary>
        /// 
        [HttpGet("get-all-district")]
        public async Task<ActionResult<ApiResult<List<LocationVm>>>> GetAllDictrict()
        {
            var result = await _locationService.GetAllSubDistrict();
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách tỉnh/thành phố
        /// </summary>
        /// 
        [HttpGet("get-all-province")]
        public async Task<ActionResult<ApiResult<List<LocationVm>>>> GetAllProvince()
        {
            var result = await _locationService.GetAllSubDistrict();
            return Ok(result);
        }
    }
}
