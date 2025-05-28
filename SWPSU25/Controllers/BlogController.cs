using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetBlogById([FromBody] Guid blogId)
        {
            var user = await _blogService.GetBlogByIdAsync(blogId);
            if (user == null)
            {
                return NotFound(new { Message = $"Blog với ID {blogId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-blog")]
        public async Task<IActionResult> GetListBlog()
        {
            var blogs = await _blogService.GetAllBlogsAsync();
            return Ok(blogs);
        }
    }
}
