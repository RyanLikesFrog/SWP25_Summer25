﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
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
        public async Task<IActionResult> GetTreatmentStageById(Guid treatmentStageId)
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

        [HttpPost("create-treatment-stage-medical-record")]
        public async Task<IActionResult> CreateTreatmentStage([FromBody] CreateTreatmentStageRequest request)
        {
            try
            {
                var result = await _treatmentStageService.CreateTreatmentStageAsync(request);
                return Ok(new { Message = "Tạo giai đoạn điều trị thành công", Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trong quá trình tạo giai đoạn điều trị.", Detail = ex.Message });
            }
        }
        [HttpPut("update-treatment-stage-medicine")]
        public async Task<IActionResult> UpdateTreatmentStageMedicine([FromBody] UpdateTreatmentStateMedicineRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedStage = await _treatmentStageService.UpdateTreatmentStageAsync(request);

                if (updatedStage == null)
                {
                    return NotFound(new { Message = $"Không tìm thấy giai đoạn điều trị với ID {request.Id}." });
                }

                return Ok(new
                {
                    Message = "Cập nhật thuốc trong giai đoạn điều trị thành công.",
                    Data = updatedStage
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Optional: log the error
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật thuốc cho giai đoạn điều trị." });
            }
        }


    }
}
