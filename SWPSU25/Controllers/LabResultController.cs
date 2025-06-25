using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.Implements;
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
        public async Task<IActionResult> GetLabResultById(Guid labResultId)
        {
            var user = await _labResultService.GetLabResultByIdAsync(labResultId);
            if (user == null)
            {
                return NotFound(new { Message = $"Kết quả phòng lab với ID {labResultId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-lab-result")]
        public async Task<IActionResult> GetListLabResult()
        {
            var labResults = await _labResultService.GetAllLabResultsAsync();
            return Ok(labResults);
        }

        [HttpPost("create-lab-result")]
        public async Task<IActionResult> CreateLabResult([FromForm] CreateLabResultRequest request)
        {
            try
            {
                var result = await _labResultService.CreateLabResultAsync(request);
                return CreatedAtAction(nameof(GetLabResultById), new { labResultId = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
