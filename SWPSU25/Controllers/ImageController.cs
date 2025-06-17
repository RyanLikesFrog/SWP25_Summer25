using Microsoft.AspNetCore.Mvc;

namespace SWPSU25.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly FirebaseStorageService _firebaseService;

        public ImageController(FirebaseStorageService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                var url = await _firebaseService.UploadFileAsync(file, "avatars"); // Tên folder trên Firebase Storage
                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage([FromQuery] string url)
        {
            try
            {
                await _firebaseService.DeleteFileAsync(url);
                return Ok(new { message = "Image deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
