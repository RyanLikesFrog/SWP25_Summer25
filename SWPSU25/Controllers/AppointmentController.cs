using DataLayer.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<MomoSettings> _momoSettings; // Cần thiết để xác thực chữ ký Momo Callback
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, IOptions<MomoSettings> momoSettings, ILogger<AppointmentController> logger)
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

        [HttpGet("get-list-appointments")]
        public async Task<IActionResult> GetListAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("get-paid-appointments")]
        public async Task<IActionResult> GetPaidAppointments()
        {
            var paidAppointments = await _appointmentService.GetPaidAppointmentsAsync();
            return Ok(paidAppointments);
        }

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

        [HttpPost("momo-callback")]
        [ProducesResponseType(200)] // Momo mong đợi response 200 OK trong mọi trường hợp
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackRequest momoCallbackRequest)
        {
            _logger.LogInformation("Momo IPN Callback received. Request: {Request}", JsonSerializer.Serialize(momoCallbackRequest));

            if (momoCallbackRequest == null || string.IsNullOrEmpty(momoCallbackRequest.OrderId) || string.IsNullOrEmpty(momoCallbackRequest.Signature))
            {
                _logger.LogError("Momo IPN Callback: Invalid or incomplete request received. OrderId: {OrderId}, Signature present: {SigPresent}",
                                 momoCallbackRequest?.OrderId, !string.IsNullOrEmpty(momoCallbackRequest?.Signature));
                return Ok(new { status = "error", message = "Invalid callback request data." });
            }

            try
            {
                var secretKey = _momoSettings.Value.SecretKey;
                var accessKey = _momoSettings.Value.AccessKey; // Lấy AccessKey từ cấu hình để xác thực

                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(accessKey))
                {
                    _logger.LogError("Momo IPN Callback: SecretKey or AccessKey is not configured.");
                    return Ok(new { status = "error", message = "Server configuration error." });
                }

                // Chuẩn bị chuỗi raw data để xác thực chữ ký (TUÂN THỦ CHÍNH XÁC TÀI LIỆU)
                // Lấy các giá trị từ request và xử lý null cho extraData và payType
                string extraDataValue = momoCallbackRequest.ExtraData ?? "";
                string payTypeValue = momoCallbackRequest.PayType ?? "";

                // Tạo danh sách các cặp key-value và sắp xếp theo alphabet
                // Dựa chính xác vào chuỗi mẫu trong tài liệu IPN Signature
                var signatureParams = new List<string>
            {
                $"accessKey={accessKey}",
                $"amount={momoCallbackRequest.Amount}",
                $"extraData={extraDataValue}",
                $"message={momoCallbackRequest.Message}",
                $"orderId={momoCallbackRequest.OrderId}",
                $"orderInfo={momoCallbackRequest.OrderInfo}",
                $"orderType={momoCallbackRequest.OrderType}",
                $"partnerCode={momoCallbackRequest.PartnerCode}",
                $"payType={payTypeValue}",
                $"requestId={momoCallbackRequest.RequestId}",
                $"responseTime={momoCallbackRequest.ResponseTime}",
                $"resultCode={momoCallbackRequest.ResultCode}",
                $"transId={momoCallbackRequest.TransId}"
            };

                // Sắp xếp các tham số theo thứ tự alphabet
                signatureParams.Sort();

                // Nối các tham số thành chuỗi rawData
                string rawDataForSignatureVerification = String.Join("&", signatureParams);

                _logger.LogDebug("Momo IPN Callback - Raw data for signature verification: {RawData}", rawDataForSignatureVerification);

                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
                {
                    byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawDataForSignatureVerification));
                    string calculatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                    if (calculatedSignature != momoCallbackRequest.Signature.ToLower())
                    {
                        _logger.LogError("Momo IPN Callback: Signature verification failed for OrderId: {OrderId}. Calculated: {CalculatedSig}, Received: {ReceivedSig}",
                                         momoCallbackRequest.OrderId, calculatedSignature, momoCallbackRequest.Signature);
                        return Ok(new { status = "error", message = "Signature verification failed." });
                    }
                }

                DateTime? momoResponseDateTime = momoCallbackRequest.ResponseTime != 0 ?
                                                 DateTimeOffset.FromUnixTimeMilliseconds(momoCallbackRequest.ResponseTime).UtcDateTime : (DateTime?)null;

                if (momoCallbackRequest.ResultCode == 0)
                {
                    _logger.LogInformation("Momo IPN Callback: Payment successful for OrderId: {OrderId}. TransId: {TransId}",
                                           momoCallbackRequest.OrderId, momoCallbackRequest.TransId);

                    await _appointmentService.UpdatePaymentStatusAsync(
                        momoCallbackRequest.OrderId,
                        PaymentTransactionStatus.Success,
                        PaymentStatus.Paid,
                        momoCallbackRequest.ResultCode,
                        momoCallbackRequest.Message,
                        momoCallbackRequest.TransId.ToString(),
                        momoCallbackRequest.RequestId,
                        momoCallbackRequest.Signature,
                        momoResponseDateTime.Value,
                        momoCallbackRequest.ExtraData
                    );
                }
                else
                {
                    _logger.LogWarning("Momo IPN Callback: Payment failed for OrderId: {OrderId}. ResultCode: {ResultCode}, Message: {Message}",
                                       momoCallbackRequest.OrderId, momoCallbackRequest.ResultCode, momoCallbackRequest.Message);

                    await _appointmentService.UpdatePaymentStatusAsync(
                        momoCallbackRequest.OrderId,
                        PaymentTransactionStatus.Failed,
                        PaymentStatus.Failed,
                        momoCallbackRequest.ResultCode,
                        momoCallbackRequest.Message,
                        momoCallbackRequest.TransId.ToString(),
                        momoCallbackRequest.RequestId,
                        momoCallbackRequest.Signature,
                        momoResponseDateTime.Value,
                        momoCallbackRequest.ExtraData
                    );
                }

                return Ok(new { status = "success", message = "IPN received and processed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing Momo IPN callback for OrderId: {OrderId}", momoCallbackRequest.OrderId);
                return Ok(new { status = "error", message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("favicon.ico")]
        public IActionResult Favicon()
        {
            return NoContent(); // Trả về HTTP 204 No Content
        }

        [HttpPut("update-appointment-online-link")]
        public async Task<IActionResult> UpdateOnlineLink([FromBody] UpdateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(request);

                if (updatedAppointment == null)
                {
                    return NotFound($"Không tìm thấy Appointment với ID {request.AppointmentId}.");
                }

                return Ok(new
                {
                    Message = "Cập nhật OnlineLink thành công.",
                    Data = updatedAppointment
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, "Đã xảy ra lỗi khi cập nhật lịch hẹn.");
            }
        }
        [HttpPut("reschedule")]
        public async Task<IActionResult> ReArrangeDateAppointment([FromBody] UpdateReArrangeDateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedAppointment = await _appointmentService.ReArrangeDateAppointmentAsync(request);

                return Ok(new
                {
                    Message = "Dời lịch hẹn thành công.",
                    Data = updatedAppointment
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi không mong muốn khi dời lịch hẹn.");
            }
        }

    }
}
