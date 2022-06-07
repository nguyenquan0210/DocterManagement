using DoctorManagement.Application.Catalog.Topic;
using DoctorManagement.ViewModels.Catalog.Topic;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        public TopicController(ITopicService TopicService)
        {
            _topicService = TopicService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// Tạo mới chủ đề
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromForm] TopicCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _topicService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa chủ đề
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
            var affectedResult = await _topicService.Delete(Id);

            return Ok(affectedResult);
        }
        /// <summary>
        /// Cập nhật chủ đề
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromForm] TopicUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _topicService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang chủ đề
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<TopicVm>>>> GetAllPaging([FromQuery] GetTopicPagingRequest request)
        {
            var result = await _topicService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy chủ đề theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<TopicVm>>> GetById(Guid Id)
        {
            var result = await _topicService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chủ đề
        /// </summary>
        /// 
        [HttpGet("get-all-topic")]
        public async Task<ActionResult<ApiResult<List<TopicVm>>>> GetAllTopic()
        {
            var Topic = await _topicService.GetAll();
            return Ok(Topic);
        }
    }
}
