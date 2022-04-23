using DoctorManagement.Application.Catalog.Speciality;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityService _specialityService;
        public SpecialityController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// Tạo mới chuyên khoa
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] SpecialityCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _specialityService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
        /// <summary>
        /// Xóa chuyên khoa
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
            var affectedResult = await _specialityService.Delete(Id);

            return Ok(affectedResult);
        }
        /// <summary>
        /// Cập nhật chuyên khoa
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] SpecialityUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _specialityService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang chuyên khoa
        /// </summary>
        /// 
        //http://localhost/api/categories/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<SpecialityVm>>>> GetAllPaging([FromQuery] GetSpecialityPagingRequest request)
        {
            var result = await _specialityService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy chuyên khoa theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<SpecialityVm>>> GetById(Guid Id)
        {
            var result = await _specialityService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find speciality");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chuyên khoa
        /// </summary>
        /// 
        [HttpGet("get-all-speciality")]
        public async Task<ActionResult<ApiResult<List<SpecialityVm>>>> GetAll()
        {
            var speciality = await _specialityService.GetAll();
            return Ok(speciality);
        }
    }
}
