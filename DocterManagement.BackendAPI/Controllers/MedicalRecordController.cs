using DoctorManagement.Application.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }
        /// <summary>
        /// Tạo mới hồ sơ bệnh án
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] MedicalRecordCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _medicalRecordService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa hồ sơ bệnh án
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
            var result = await _medicalRecordService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật hồ sơ bệnh án
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] MedicalRecordUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _medicalRecordService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang của hồ sơ bệnh án
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetMedicalRecordPagingRequest request)
        {
            var user = await _medicalRecordService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy hồ sơ bệnh theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _medicalRecordService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách hồ sơ bệnh
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _medicalRecordService.GetAll();
            return Ok(result);
        }
    }
}
