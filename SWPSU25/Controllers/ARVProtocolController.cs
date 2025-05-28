using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetARVProtocolById([FromBody] Guid protocolId)
        {
            var user = await _aRVProtocolService.GetARVProtocolByIdAsync(protocolId);
            if (user == null)
            {
                return NotFound(new { Message = $"Phác thảo với ID {protocolId} không tìm thấy." });
            }
            return Ok(user);
        }
    }
}
