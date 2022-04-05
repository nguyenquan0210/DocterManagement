using DoctorManagement.Application.Catalog.Rate;
using DoctorManagement.ViewModels.Catalog.Rate;
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
        public async Task<IActionResult> Create([FromBody] RateCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa đánh giá
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
            var result = await _rateService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật đánh giá
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RateUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang đánh giá
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetRatePagingRequest request)
        {
            var user = await _rateService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy đánh giá theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _rateService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách đánh giá
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rateService.GetAll();
            return Ok(result);
        }
    }
}