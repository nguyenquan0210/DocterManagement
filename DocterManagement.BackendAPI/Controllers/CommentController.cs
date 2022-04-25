using DoctorManagement.Application.Catalog.Comment;
using DoctorManagement.ViewModels.Catalog.Comment;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService CommentService)
        {
            _commentService = CommentService;
        }
        /// <summary>
        /// Tạo mới bình luận
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] CommentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _commentService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
        /// <summary>
        /// Xóa bình luận
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
            var result = await _commentService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật bình luận
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] CommentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _commentService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách bình luận phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<CommentVm>>>> GetAllPaging([FromQuery] GetCommentPagingRequest request)
        {
            var user = await _commentService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy dữ liệu bình luận theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<CommentVm>>> GetById(Guid Id)
        {
            var result = await _commentService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest("Cannot find comment");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách bình luân
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<CommentVm>>>> GetAll()
        {
            var result = await _commentService.GetAll();
            return Ok(result);
        }
    }
}
