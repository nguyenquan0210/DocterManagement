using DoctorManagement.Application.Catalog.Servicce;
using DoctorManagement.ViewModels.Catalog.Service;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServicesService _serviceService;
        public ServiceController(IServicesService serviceService)
        {
            _serviceService = serviceService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// Tạo mới dịch vụ khám bênh
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] ServiceCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _serviceService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa dịch vụ khám bênh
        /// </summary>
        /// 
        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> Delete([FromRoute] Guid Id)
        {
            var affectedResult = await _serviceService.Delete(Id);

            return Ok(affectedResult);
        }
        /// <summary>
        /// Cập nhật dịch vụ khám bênh
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] ServiceUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _serviceService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang dịch vụ khám bênh
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<ServiceVm>>>> GetAllPaging([FromQuery] GetServicePagingRequest request)
        {
            var result = await _serviceService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy dịch vụ khám bênh theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<ServiceVm>>> GetById(Guid Id)
        {
            var result = await _serviceService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách dịch vụ khám bênh
        /// </summary>
        /// 
        [HttpGet("get-all-service/{UserName}")]
        public async Task<ActionResult<ApiResult<List<ServiceVm>>>> GetAllService(string UserName)
        {
            var Service = await _serviceService.GetAll(UserName);
            return Ok(Service);
        }
    }
}
