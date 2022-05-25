using DoctorManagement.Application.Catalog.Contact;
using DoctorManagement.ViewModels.Catalog.Contact;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService ContactService)
        {
            _contactService = ContactService;
        }
        /// <summary>
        /// Tạo mới lịch khám
        /// </summary>
        /// 
        [HttpPost]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] ContactCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _contactService.CreateContact(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa lịch khám
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
            var result = await _contactService.DeleteContact(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật lịch khám
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] ContactUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _contactService.UpdateContact(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang lịch khám 
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<ContactVm>>>> GetAllPaging([FromQuery] GetContactPagingRequest request)
        {
            var result = await _contactService.GetAllPagingContact(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy lịch khám theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<ContactVm>>> GetById(Guid Id)
        {
            var result = await _contactService.GetByIdContact(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find Contact");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách lịch khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<ContactVm>>>> GetAll()
        {
            var result = await _contactService.GetAllContact();
            return Ok(result);
        }
       
    }
}
