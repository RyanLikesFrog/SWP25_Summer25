using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentStageController : ControllerBase
    {
        private readonly ITreatmentStageService _treatmentStageService;

        public TreatmentStageController(ITreatmentStageService treatmentStageService)
        {
            _treatmentStageService = treatmentStageService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetTreatmentStageById([FromBody] Guid treatmentStageId)
        {
            var user = await _treatmentStageService.GetTreatmentStagebyIdAsync(treatmentStageId);
            if (user == null)
            {
                return NotFound(new { Message = $"Giai đoạn điều trị với ID {treatmentStageId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-treatment-stage")]
        public async Task<IActionResult> GetListTreatmentStage()
        {
            var treatmentStages = await _treatmentStageService.GetAllTreatmentStagesAsync();
            return Ok(treatmentStages);
        }
    }
}
