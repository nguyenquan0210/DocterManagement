using DoctorManagement.Application.Catalog.Clinic;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
    public class ClinicController : ControllerBase
    {
        private readonly IClinicService _clinicService;
        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }
        /// <summary>
        /// Tạo mới phòng khám
        /// </summary>
        /// 
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromForm] ClinicCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _clinicService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa phòng khám
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
            var result = await _clinicService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Xóa hình ảnh phòng khám
        /// </summary>
        /// 
        [HttpDelete("images/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> DeleteImageClinic([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _clinicService.DeleteImg(Id);

            return Ok(result);
        }
        /// <summary>
        /// Xóa tất cả hình ảnh phòng khám
        /// </summary>
        /// 
        [HttpDelete("{Id}/delete-all-images")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> DeleteAllImageClinic([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _clinicService.DeleteAllImg(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật phòng khám
        /// </summary>
        /// 
        [Authorize]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromForm] ClinicUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _clinicService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách phòng khám phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<ClinicVm>>>> GetAllPaging([FromQuery] GetClinicPagingRequest request)
        {
            var result = await _clinicService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy dữ liệu phòng khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<ClinicVm>>> GetById(Guid Id)
        {
            var result = await _clinicService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find clinic");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách phòng khám
        /// </summary>
        /// 
        [HttpGet("get-all-clinic")]
        public async Task<ActionResult<ApiResult<List<ClinicVm>>>> GetAll()
        {
            var result = await _clinicService.GetAll();
            return Ok(result);
        }
    }
}
