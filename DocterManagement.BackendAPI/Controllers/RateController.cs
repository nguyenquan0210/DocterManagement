using DoctorManagement.Application.Catalog.Rate;
using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;
        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }
       
        /// <summary>
        /// Tạo mới đánh giá
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create(RateCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(request);

            return Ok(result);
        }

        /// <summary>
        /// Xóa đánh giá
        /// </summary>
        /// 
        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> Delete([FromRoute] Guid Id)
        {
            var result = await _rateService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật đánh giá
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] RateUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang đánh giá
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<RatesVm>>>> GetAllPaging([FromQuery] GetRatePagingRequest request)
        {
            var result = await _rateService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy đánh giá theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<RatesVm>>> GetById(Guid Id)
        {
            var result = await _rateService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách đánh giá
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<RatesVm>>>> GetAll()
        {
            var result = await _rateService.GetAll();
            return Ok(result);
        }
       
    }
}