using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
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
        public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
        {
            var user = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (user == null)
            {
                return NotFound(new { Message = $"Cuộc hẹn với ID {appointmentId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-appointment")]
        public async Task<IActionResult> GetListAppointment()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpPost("patient-create-appointment")]
        public async Task<IActionResult> RegisterAppointment(Guid patientId, [FromBody] UserCreateAppointmentRequest request)
        {
            if (patientId != request.PatientId)
            {
                return BadRequest("Patient ID in URL does not match Patient ID in request body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _appointmentService.RegisterAppointmentAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                // Patient not found or Doctor not found
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Doctor not available or other business logic errors
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while registering the appointment.", error = ex.Message });
            }
        }
        [HttpPost("staff-manages-appointment")]
        public async Task<IActionResult> StaffManagesAppointment([FromBody] StaffManageAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _appointmentService.StaffUpdateAppointmentAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                // Patient not found or Doctor not found
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Doctor not available or other business logic errors
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while registering the appointment.", error = ex.Message });
            }
        }
    }
}
