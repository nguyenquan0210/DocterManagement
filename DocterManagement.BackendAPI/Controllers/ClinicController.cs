using DoctorManagement.Application.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Clinic;
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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ClinicCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _clinicService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa phòng khám
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
            var result = await _clinicService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật phòng khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ClinicUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _clinicService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }

        /// <summary>
        /// Lấy danh sách phòng khám phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetClinicPagingRequest request)
        {
            var user = await _clinicService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy dữ liệu phòng khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _clinicService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách phòng khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clinicService.GetAll();
            return Ok(result);
        }
    }
}
