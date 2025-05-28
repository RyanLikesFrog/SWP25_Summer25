using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientTreatmentProtocolController : ControllerBase
    {
        private readonly IPatientTreatmentProtocolService _patientTreatmentProtocolService;

        public PatientTreatmentProtocolController(IPatientTreatmentProtocolService patientTreatmentProtocolService)
        {
            _patientTreatmentProtocolService = patientTreatmentProtocolService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetPatientTreatmentProtocolById(Guid patientTreatmentProtocolId)
        {
            var user = await _patientTreatmentProtocolService.GetPatientTreatmentProtocolByIdAsync(patientTreatmentProtocolId);
            if (user == null)
            {
                return NotFound(new { Message = $"Phác đồ điều trị bệnh nhân với ID {patientTreatmentProtocolId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-patient-treatment-protocol")]
        public async Task<IActionResult> GetListPatientTreatmentProtocol()
        {
            var patientTreatmentProtocols = await _patientTreatmentProtocolService.GetAllPatientTreatmentProtocolsAsync();
            return Ok(patientTreatmentProtocols);
        }
    }
}
