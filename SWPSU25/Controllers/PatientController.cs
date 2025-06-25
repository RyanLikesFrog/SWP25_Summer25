using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IUserService _userService;

        public PatientController(IPatientService patientService, IUserService userService)
        {
            _patientService = patientService;
            _userService = userService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetPatientById(Guid patientId)
        {
            var user = await _patientService.GetPatientByUserIdAsync(patientId);
            if (user == null)
            {
                return NotFound(new { Message = $"Bệnh nhân với ID {patientId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-patient")]
        public async Task<IActionResult> GetListPatient()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }
        [HttpPut("patient-update-profile")]
        public async Task<IActionResult> UpdatePatientProfile([FromForm] UpdatePatientRequest request)
        {
            if (!ModelState.IsValid) // Kiểm tra Data Annotations validation
            {
                return BadRequest(ModelState);
            }

            var result = await _patientService.UpdatePatientProfileAsync(request);

            if (!result.Success)
            {
                return StatusCode(500, new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, UserId = result.Patient?.Id });
        }

    }
}
