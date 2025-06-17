using DataLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetDoctorById(Guid doctorId)
        {
            var user = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (user == null)
            {
                return NotFound(new { Message = $"Bác sĩ với ID {doctorId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-doctor")]
        public async Task<IActionResult> GetListDoctor()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [HttpPut("update-doctor")]
        public async Task<IActionResult> UpdateDoctor([FromBody] UpdateDoctorRequest request)
        {
            try
            {
                var updatedDoctor = await _doctorService.UpdateDoctorAsync(request);
                if (updatedDoctor == null)
                {
                    return NotFound(new { Message = $"Không tìm thấy bác sĩ với ID {request.DoctorId}" });
                }
                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật bác sĩ.", Error = ex.Message });
            }
        }


    }
}
