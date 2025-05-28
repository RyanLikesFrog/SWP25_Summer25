using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetDoctorById([FromBody] Guid doctorId)
        {
            var user = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (user == null)
            {
                return NotFound(new { Message = $"Bác sĩ với ID {doctorId} không tìm thấy." });
            }
            return Ok(user);
        }
    }
}
