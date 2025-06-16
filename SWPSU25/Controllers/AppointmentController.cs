using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;
using ServiceLayer.PaymentGateways;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly VnPayService _vnPayService;
        public AppointmentController(IAppointmentService appointmentService, VnPayService vnPayService)
        {
            _appointmentService = appointmentService;
            _vnPayService = vnPayService;
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
        {
            var user = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (user == null)
            {
                return NotFound(new { Message = $"Cuộc hẹn với ID {appointmentId} không tìm thấy." });
            }
            return Ok(user);
        }

        [HttpGet("get-list-appointment")]
        public async Task<IActionResult> GetListAppointment()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        //[HttpPost("patient-create-appointment")]
        //public async Task<IActionResult> RegisterAppointment(Guid patientId, [FromBody] UserCreateAppointmentRequest request)
        //{
        //    if (patientId != request.PatientId)
        //    {
        //        return BadRequest("Patient ID in URL does not match Patient ID in request body.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var response = await _appointmentService.RegisterAppointmentAsync(request);
        //        return Ok(response);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        // Patient not found or Doctor not found
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Doctor not available or other business logic errors
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while registering the appointment.", error = ex.Message });
        //    }
        //}
        //[HttpPost("staff-manages-appointment")]
        //public async Task<IActionResult> StaffManagesAppointment([FromBody] StaffManageAppointmentRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    try
        //    {
        //        var response = await _appointmentService.StaffUpdateAppointmentAsync(request);
        //        return Ok(response);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        // Patient not found or Doctor not found
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Doctor not available or other business logic errors
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while registering the appointment.", error = ex.Message });
        //    }
        //}

        [HttpGet("doctor-get-appointments")]
        public async Task<IActionResult> ViewAppointments([FromQuery] Guid doctorId)
        {
            var (appointments, message) = await _appointmentService.ViewAppointmentAsync(doctorId);

            if (appointments == null)
            {
                return NotFound(new { Message = message });
            }

            return Ok(new { Message = message, Appointments = appointments });
        }
        [HttpPost("patient-create-appointment-and-pay")] // Endpoint mới cho việc đặt lịch và thanh toán ngay
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAppointmentAndPay([FromBody] UserCreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _appointmentService.CreateAppointmentAndInitiatePaymentAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during appointment booking and payment initiation. Please try again later." });
            }
        }

        // Endpoint cho VNPAY NotifyUrl (Webhook/IPN) - Đã có từ trước nhưng nhắc lại tầm quan trọng
        [HttpGet("vnpay-callback")] // VNPAY thường gửi IPN qua GET
        [Produces("application/json")]
        public async Task<IActionResult> VnPayCallback()
        {
            var queryParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

            string rspCode = "99";
            string message = "Unknown error";

            try
            {
                await _vnPayService.ProcessVnPayCallbackAsync(queryParams);
                rspCode = "00"; // Success
                message = "Confirm Success";
            }
            catch (InvalidOperationException ex) // Lỗi do SecureHash không hợp lệ
            {
                rspCode = "97";
                message = ex.Message;
            }
            catch (KeyNotFoundException ex) // Lỗi không tìm thấy giao dịch/appointment
            {
                rspCode = "01"; // Giao dịch không tồn tại
                message = ex.Message;
            }
            catch (Exception ex) // Lỗi tổng quát khác
            {
                rspCode = "99";
                message = "An unexpected error occurred.";
            }

            // VNPAY yêu cầu phản hồi JSON với RspCode và Message
            return Ok(new { RspCode = rspCode, Message = message });
        }
    }
}
