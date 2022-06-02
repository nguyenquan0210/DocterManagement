using DoctorManagement.Application.Catalog.Medicine;
using DoctorManagement.ViewModels.Catalog.Medicine;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;
        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// Tạo mới kệ thuốc
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromForm] MedicineCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _medicineService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa kệ thuốc
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
            var affectedResult = await _medicineService.Delete(Id);

            return Ok(affectedResult);
        }
        /// <summary>
        /// Cập nhật kệ thuốc
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromForm] MedicineUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _medicineService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang kệ thuốc
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<MedicineVm>>>> GetAllPaging([FromQuery] GetMedicinePagingRequest request)
        {
            var result = await _medicineService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy kệ thuốc theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<MedicineVm>>> GetById(Guid Id)
        {
            var result = await _medicineService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách kệ thuốc
        /// </summary>
        /// 
        [HttpGet("get-all-medicine/{UserName}")]
        public async Task<ActionResult<ApiResult<List<MedicineVm>>>> GetAllMedicine(string UserName)
        {
            var Medicine = await _medicineService.GetAll(UserName);
            return Ok(Medicine);
        }
    }
}
