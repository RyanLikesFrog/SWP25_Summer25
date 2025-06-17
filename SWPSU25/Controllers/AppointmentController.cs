using DataLayer.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.DTOs.Payment;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;
using ServiceLayer.PaymentGateways;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly MomoSettings _momoSettings; // Cần thiết để xác thực chữ ký Momo Callback
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, MomoSettings momoSettings, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _momoSettings = momoSettings;
            _logger = logger;
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
        
        /// <summary>
        /// Tạo một cuộc hẹn mới và khởi tạo quá trình thanh toán Momo.
        /// </summary>
        /// <param name="request">Thông tin chi tiết về cuộc hẹn.</param>
        /// <returns>Thông tin cuộc hẹn đã tạo và URL chuyển hướng/QR Code URL.</returns>
        [HttpPost("create-and-initiate-payment")]
        [ProducesResponseType(typeof(AppointmentDetailResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CreateAppointmentAndInitiatePayment([FromBody] UserCreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateAppointmentAndInitiatePayment: {Errors}",
                                   JsonSerializer.Serialize(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _appointmentService.CreateAppointmentAndInitiatePaymentAsyncV2(request);
                _logger.LogInformation("Successfully created appointment and initiated payment for AppointmentId: {AppointmentId}", response.Id);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error during appointment creation: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating appointment and initiating payment.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Endpoint xử lý Callback (IPN) từ Momo khi giao dịch hoàn tất hoặc thất bại.
        /// </summary>
        /// <param name="momoCallbackRequest">Dữ liệu callback từ Momo.</param>
        /// <returns>HTTP 200 OK nếu xử lý thành công, lỗi nếu xác thực hoặc xử lý thất bại.</returns>
        [HttpPost("momo-callback")]
        [ProducesResponseType(200)] // Momo mong đợi response 200 OK
        [ProducesResponseType(400)] // Lỗi nếu request không hợp lệ
        [ProducesResponseType(500)] // Lỗi nội bộ server
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackRequest momoCallbackRequest)
        {
            _logger.LogInformation("Momo Callback received. Request: {Request}", JsonSerializer.Serialize(momoCallbackRequest));

            // Kiểm tra xem dữ liệu callback có hợp lệ không (ví dụ: null)
            if (momoCallbackRequest == null || string.IsNullOrEmpty(momoCallbackRequest.OrderId) || string.IsNullOrEmpty(momoCallbackRequest.Signature))
            {
                _logger.LogError("Momo Callback: Invalid or incomplete request received.");
                return BadRequest("Invalid callback request.");
            }

            try
            {
                // 1. Xác thực chữ ký (Signature) của Momo callback
                // Chuỗi raw data cần phải TUÂN THỦ CHÍNH XÁC TÀI LIỆU MOMO cho callback verification
                // Các tham số cần được nối theo thứ tự alphabet hoặc thứ tự quy định của Momo.
                // Đảm bảo thứ tự và tên các trường khớp với quy tắc băm của Momo cho IPN.
                string rawDataForSignatureVerification = $"partnerCode={momoCallbackRequest.PartnerCode}&accessKey={_momoSettings.AccessKey}&requestId={momoCallbackRequest.RequestId}&orderId={momoCallbackRequest.OrderId}&amount={momoCallbackRequest.Amount}&message={momoCallbackRequest.Message}&resultCode={momoCallbackRequest.ResultCode}&responseTime={momoCallbackRequest.ResponseTime}&extraData={momoCallbackRequest.ExtraData}";

                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_momoSettings.SecretKey)))
                {
                    byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawDataForSignatureVerification));
                    string calculatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                    if (calculatedSignature != momoCallbackRequest.Signature.ToLower())
                    {
                        _logger.LogError("Momo Callback: Signature verification failed for OrderId: {OrderId}. Calculated: {CalculatedSig}, Received: {ReceivedSig}",
                                         momoCallbackRequest.OrderId, calculatedSignature, momoCallbackRequest.Signature);
                        return BadRequest("Signature verification failed.");
                    }
                }

                // Chuyển đổi ResponseTime từ long epoch milliseconds sang DateTime (UTC)
                DateTime? momoResponseDateTime = momoCallbackRequest.ResponseTime != 0 ?
                                                DateTimeOffset.FromUnixTimeMilliseconds(momoCallbackRequest.ResponseTime).UtcDateTime : (DateTime?)null;

                // 2. Xử lý kết quả giao dịch dựa trên ResultCode
                if (momoCallbackRequest.ResultCode == 0) // ResultCode = 0 thường là thành công
                {
                    _logger.LogInformation("Momo Callback: Payment successful for OrderId: {OrderId}. TransId: {TransId}",
                                           momoCallbackRequest.OrderId, momoCallbackRequest.TransId);

                    await _appointmentService.UpdatePaymentStatusAsync(
                        momoCallbackRequest.OrderId,
                        PaymentTransactionStatus.Success,
                        PaymentStatus.Paid,
                        momoCallbackRequest.ResultCode,
                        momoCallbackRequest.Message,
                        momoCallbackRequest.TransId,
                        momoCallbackRequest.RequestId,
                        momoCallbackRequest.Signature,
                        momoResponseDateTime,
                        momoCallbackRequest.ExtraData
                    );
                }
                else
                {
                    _logger.LogWarning("Momo Callback: Payment failed for OrderId: {OrderId}. ResultCode: {ResultCode}, Message: {Message}",
                                       momoCallbackRequest.OrderId, momoCallbackRequest.ResultCode, momoCallbackRequest.Message);

                    await _appointmentService.UpdatePaymentStatusAsync(
                        momoCallbackRequest.OrderId,
                        PaymentTransactionStatus.Failed,
                        PaymentStatus.Failed,
                        momoCallbackRequest.ResultCode,
                        momoCallbackRequest.Message,
                        momoCallbackRequest.TransId,
                        momoCallbackRequest.RequestId,
                        momoCallbackRequest.Signature,
                        momoResponseDateTime,
                        momoCallbackRequest.ExtraData
                    );
                }

                // Momo mong đợi HTTP 200 OK để xác nhận đã nhận và xử lý callback
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing Momo callback for OrderId: {OrderId}", momoCallbackRequest.OrderId);
                // Trả về lỗi 500 hoặc 400 nếu có lỗi trong quá trình xử lý để Momo có thể thử lại (nếu cấu hình)
                return StatusCode(500, "Internal server error during callback processing.");
            }
        }
    }
}
