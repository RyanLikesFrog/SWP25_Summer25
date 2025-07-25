﻿using Microsoft.AspNetCore.Authorization;
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
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IDoctorScheduleService _doctorScheduleService;

        public DoctorScheduleController(IDoctorScheduleService doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetDoctorScheduleById(Guid scheduleId)
        {
            var user = await _doctorScheduleService.GetDoctorSchedulebyIdAsync(scheduleId);
            if (user == null)
            {
                return NotFound(new { Message = $"Thời khóa biểu với ID {scheduleId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-doctor-schedule")]
        public async Task<IActionResult> GetListDoctorSchedule()
        {
            var doctorSchedules = await _doctorScheduleService.GetAllDoctorSchedulesAsync();
            return Ok(doctorSchedules);
        }

        [HttpGet("get-list-doctor-schedule-by-startdate-enddate")]
        public async Task<IActionResult> GetDuplicatedDoctorScheduleByStartDateEndDate(Guid doctorId, DateTime StartTime, DateTime? EndTime)
        {
            var doctorSchedules = await _doctorScheduleService.GetDuplicatedDoctorScheduleByStartDateEndDateAsync(doctorId, StartTime, EndTime);
            return Ok(doctorSchedules);
        }

        [HttpGet("doctor-get-schedule-by-id")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ViewDoctorSchedule([FromQuery] Guid doctorId)
        {
            var (schedules, message) = await _doctorScheduleService.ViewDoctorScheduleAsync(doctorId);

            if (schedules == null)
            {
                return NotFound(new { Message = message });
            }

            return Ok(new { Message = message, Schedules = schedules });
        }

        [HttpGet("get-today-list-doctor-schedule-by-doctor-id")]
        public async Task<IActionResult> ViewTodayDoctorSchedule([FromQuery] Guid doctorId)
        {
            var doctorSchedules = await _doctorScheduleService.GetTodayDoctorScheduleByDoctorIdAsync(doctorId);

            return Ok(doctorSchedules);
        }

        [HttpPost("create-doctor-schedule")]
        public async Task<IActionResult> CreateDoctorSchedule([FromBody] CreateDoctorScheduleRequest request)
        {
            try
            {
                var result = await _doctorScheduleService.CreateDoctorScheduleAsync(request);
                return Ok(new
                {
                    Message = "Tạo lịch làm việc bác sĩ thành công.",
    
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Đã xảy ra lỗi trong quá trình tạo lịch làm việc bác sĩ.",
                    Details = ex.Message
                });
            }
        }
        [HttpPut("update-doctor-schedule")]
        public async Task<IActionResult> UpdateDoctorSchedule([FromBody] UpdateDoctorScheduleRequest request)
        {
            try
            {
                var updatedSchedule = await _doctorScheduleService.UpdateDoctorScheduleAsync(request);

                if (updatedSchedule == null)
                {
                    return NotFound(new { Message = $"Không tìm thấy lịch làm việc với ID {request.Id}." });
                }

                return Ok(new
                {
                    Message = "Cập nhật lịch làm việc bác sĩ thành công.",
                    Data = updatedSchedule
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Đã xảy ra lỗi trong quá trình cập nhật lịch làm việc bác sĩ.",
                    Details = ex.Message
                });
            }
        }

    }
}
