﻿using DoctorManagement.Application.Catalog.Comment;
using DoctorManagement.ViewModels.Catalog.Comment;
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
        public async Task<IActionResult> Create([FromBody] CommentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _commentService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        /// <summary>
        /// Xóa bình luận
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
            var result = await _commentService.Delete(Id);

            return Ok(result);
        }
        /// <summary>
        /// Cập nhật bình luận
        /// </summary>
        /// 
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] CommentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _commentService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }

        /// <summary>
        /// Lấy danh sách bình luận phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetCommentPagingRequest request)
        {
            var user = await _commentService.GetAllPaging(request);
            return Ok(user);
        }
        /// <summary>
        /// Lấy dữ liệu bình luận theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _commentService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách bình luân
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _commentService.GetAll();
            return Ok(result);
        }
    }
}