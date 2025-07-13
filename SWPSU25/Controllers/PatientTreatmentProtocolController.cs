using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
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

        [HttpPost("create-patient-treatment-protocol")]
        public async Task<IActionResult> CreatePatientTreatmentProtocol([FromBody] CreatePatientTreatmentProtocolRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _patientTreatmentProtocolService.CreatePatientTreatmentProtocolAsync(request);
                if (response != null)
                {
                    return CreatedAtAction(nameof(GetPatientTreatmentProtocolById), new { id = response.Id }, response);
                }
                else
                {
                    // Trường hợp service trả về null: có thể là lỗi DB hoặc lỗi không xác định
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể tạo phác đồ điều trị bệnh nhân do lỗi nội bộ hoặc lỗi cơ sở dữ liệu." });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update-treatment-protocol-status")]
        public async Task<IActionResult> UpdatePatientTreatmentProtocolStatus([FromBody] UpdatePatientTreatmentProtocolStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var success = await _patientTreatmentProtocolService.UpdatePatientTreatmentProtocolStatusAsync(request);

            if (success)
            {
                return Ok(new { Message = $"Cập nhật trạng thái phác đồ {request.ProtocolId} thành công." });
            }
            else
            {
                return NotFound(new { Message = $"Không tìm thấy phác đồ điều trị với ID {request.ProtocolId} hoặc cập nhật thất bại." });
            }
        }
    }
}
