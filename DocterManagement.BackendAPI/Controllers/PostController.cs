using DoctorManagement.Application.Catalog.Post;
using DoctorManagement.ViewModels.Catalog.Post;
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
        public async Task<IActionResult> Create([FromBody] PostCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa bài viết
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
            var result = await _postService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] PostUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Lấy danh sách phân trang bài viết
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetPostPagingRequest request)
        {
            var user = await _postService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy bài viết theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _postService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách bài viết
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _postService.GetAll();
            return Ok(result);
        }
    }
}
