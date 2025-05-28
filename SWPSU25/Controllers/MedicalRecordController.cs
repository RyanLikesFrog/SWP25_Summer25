using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecord)
        {
            _medicalRecordService = medicalRecord;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetMedicalRecordById(Guid medicalRecordId)
        {
            var user = await _medicalRecordService.GetMedicalRecordByIdAsync(medicalRecordId);
            if (user == null)
            {
                return NotFound(new { Message = $"Hồ sơ bệnh án với ID {medicalRecordId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-medical-record")]
        public async Task<IActionResult> GetListMedicalRecord()
        {
            var medicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
            return Ok(medicalRecords);
        }
    }
}
