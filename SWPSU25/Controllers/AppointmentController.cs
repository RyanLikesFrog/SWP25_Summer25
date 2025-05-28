using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetAppointmentById([FromBody] Guid appointmentId)
        {
            var user = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (user == null)
            {
                return NotFound(new { Message = $"Cuộc hẹn với ID {appointmentId} không tìm thấy." });
            }
            return Ok(user);
        }
    }
}
