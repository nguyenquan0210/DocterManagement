using DoctorManagement.Application.Catalog.Post;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController  : ControllerBase
    {
        private readonly IPostService _postService;
        public StorageController(IPostService postService)
        {
            _postService = postService;
        }
        /// <summary>
        /// Thêm ảnh
        /// </summary>
        /// 
        [HttpPost("images")]
        public async Task<ActionResult<ApiResult<ImagesVm>>> Create([FromForm] ImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.AddImage(request);
            if (!result.IsSuccessed)
                return BadRequest();

            return Ok(result);
        }
    }
}
