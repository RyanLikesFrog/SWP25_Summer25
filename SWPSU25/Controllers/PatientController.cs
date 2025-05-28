using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetPatientById([Guid patientId)
        {
            var user = await _patientService.GetPatientByIdAsync(patientId);
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
    }
}
