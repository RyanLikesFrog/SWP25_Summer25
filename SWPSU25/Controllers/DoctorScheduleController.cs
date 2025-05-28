using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IDoctorScheduleService _doctorScheduleService;

        public DoctorScheduleController(IDoctorScheduleService doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetDoctorScheduleById([FromBody] Guid scheduleId)
        {
            var user = await _doctorScheduleService.GetDoctorSchedulebyIdAsync(scheduleId);
            if (user == null)
            {
                return NotFound(new { Message = $"Thời khóa biểu với ID {scheduleId} không tìm thấy." });
            }
            return Ok(user);
        }
    }
}
