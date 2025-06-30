using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetMedicalRecordById([FromQuery] Guid medicalRecordId)
        {
            var record = await _medicalRecordService.GetMedicalRecordByIdAsync(medicalRecordId);
            if (record == null)
            {
                return NotFound(new { Message = $"Medical record with ID {medicalRecordId} not found." });
            }
            return Ok(record);
        }

        [HttpGet("get-list-medical-record")]
        public async Task<IActionResult> GetListMedicalRecord()
        {
            var medicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
            return Ok(medicalRecords);
        }

        [HttpPut("update-medical-record")]
        public async Task<IActionResult> UpdateMedicalRecord([FromForm] UpdateMedicalRecord request)
        {
            try
            {
                var updatedRecord = await _medicalRecordService.UpdateMedicalRecordAsync(request);
                if (updatedRecord == null)
                {
                    return NotFound($"Medical record with ID {request.MedicalRecordId} not found.");
                }

                return Ok(updatedRecord);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
