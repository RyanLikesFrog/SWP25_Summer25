using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ARVProtocolController : ControllerBase
    {
        private readonly IARVProtocolService _aRVProtocolService;

        public ARVProtocolController(IARVProtocolService aRVProtocolService)
        {
            _aRVProtocolService = aRVProtocolService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetARVProtocolById(Guid protocolId)
        {
            var user = await _aRVProtocolService.GetARVProtocolByIdAsync(protocolId);
            if (user == null)
            {
                return NotFound(new { Message = $"Phác thảo với ID {protocolId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-arv-protocol")]
        public async Task<IActionResult> GetListARVProtocol()
        {
            var arvProtocols = await _aRVProtocolService.GetAllARVProtocolsAsync();
            return Ok(arvProtocols);
        }

        [HttpPost("manager-create-default-arv-protocol")]
        public async Task<IActionResult> CreateARVProtocol([FromBody] CreateARVProtocolRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _aRVProtocolService.CreateARVProtocolAsync(request);

                if (response != null)
                {
                    return CreatedAtAction(nameof(GetARVProtocolById), new { id = response.ProtocolId }, response);
                }
                else
                {
                    // Trường hợp service trả về null: có thể là lỗi DB hoặc lỗi không xác định
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể tạo ARV Protocol do lỗi nội bộ hoặc lỗi cơ sở dữ liệu." });
                }
            }
            catch (ArgumentException ex)
            {
                // Dùng cho các lỗi liên quan đến tham số đầu vào không hợp lệ (ngoài ModelState)
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (InvalidOperationException ex)
            {
                // Dùng cho các lỗi nghiệp vụ (ví dụ: tên protocol đã tồn tại)
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Log exception (cần triển khai hệ thống logging thực tế)
                Console.WriteLine($"Lỗi không mong muốn khi tạo ARV Protocol: {ex.Message}");
                // Trả về lỗi 500 cho các trường hợp không được xử lý cụ thể
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi không mong muốn khi tạo ARV Protocol.", error = ex.Message });
            }
        }

        [HttpGet("get-default-arv-protocol")]
        public async Task<IActionResult> GetDefaultARVProtocolsAsync()
        {
            var defaultProtocols = await _aRVProtocolService.GetDefaultARVProtocolsAsync();
            return Ok(defaultProtocols);
        }

    }
}
