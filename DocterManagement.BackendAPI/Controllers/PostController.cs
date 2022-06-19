using DoctorManagement.Application.Catalog.Post;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        /// <summary>
        /// Tạo mới bài viết
        /// </summary>
        /// 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromForm] PostCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa bài viết từ bác sĩ
        /// </summary>
        /// 
        [HttpDelete("deleted-doctor/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> Delete([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.Delete(Id,true);

            return Ok(result);
        }
        /// <summary>
        /// Xóa bài viết từ admin
        /// </summary>
        /// 
        [HttpDelete("deleted-admin/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> DeleteAdmin([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.Delete(Id,false);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromForm] PostUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang bài viết
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<PostVm>>>> GetAllPaging([FromQuery] GetPostPagingRequest request)
        {
            var result = await _postService.GetAllPaging(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang bài viết từ admin
        /// </summary>
        /// 
        [HttpGet("admin/paging")]
        public async Task<ActionResult<ApiResult<PagedResult<PostVm>>>> GetAllPagingPostAdmin([FromQuery] GetPostPagingRequest request)
        {
            var result = await _postService.GetAllPagingAdmin(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy bài viết theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<PostVm>>> GetById(Guid Id)
        {
            var result = await _postService.GetById(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách bài viết
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<PostVm>>>> GetAll()
        {
            var result = await _postService.GetAll();
            return Ok(result);
        }
    }
}
