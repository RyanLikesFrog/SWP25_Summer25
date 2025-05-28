using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabResultController : ControllerBase
    {
        private readonly ILabResultService _labResultService;

        public LabResultController(ILabResultService labResultService)
        {
            _labResultService = labResultService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetLabResultById([FromBody] Guid labResultId)
        {
            var user = await _labResultService.GetLabResultByIdAsync(labResultId);
            if (user == null)
            {
                return NotFound(new { Message = $"Kết quả phòng lab với ID {labResultId} không tìm thấy." });
            }
            return Ok(user);
        }
    }
}
