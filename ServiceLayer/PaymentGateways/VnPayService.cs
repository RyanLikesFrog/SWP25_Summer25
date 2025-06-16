using DataLayer.DbContext;
using DataLayer.Entities;
using DataLayer.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoLayer.Interfaces;
using ServiceLayer.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceLayer.PaymentGateways
{
    public class VnPayService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IRepository _repository;
        private readonly SWPSU25Context _context;
        private readonly VnPayPaymentClient _vnPayClient;
        private readonly ILogger<AppointmentService> _logger;
        public VnPayService(
            IAppointmentRepository appointmentRepository,
            IRepository repository,
            IDoctorScheduleRepository doctorScheduleRepository,
            SWPSU25Context context,
            VnPayPaymentClient vnPayClient,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _repository = repository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _context = context;
            _vnPayClient = vnPayClient;
            _logger = logger;
        }
        
        // Phương thức xử lý Callback/IPN từ VNPAY - **Đây là nơi tạo DoctorSchedule**
        public async Task ProcessVnPayCallbackAsync(Dictionary<string, string> queryParams)
        {
            _logger.LogInformation("Processing VNPAY Callback. Raw Data: {Data}", JsonSerializer.Serialize(queryParams));

            var vnpayResult = _vnPayClient.ProcessPaymentResult(queryParams);

            if (!vnpayResult.SecureHashValid)
            {
                _logger.LogWarning("VNPAY Callback: Invalid SecureHash for OrderId: {OrderId}", vnpayResult.OrderId);
                throw new InvalidOperationException("Invalid VNPAY SecureHash.");
            }

            // Tìm giao dịch PaymentTransaction tương ứng trong DB bằng OrderId (vnp_TxnRef)
            var appointment = await _context.Appointments
                                 .Include(a => a.PaymentTransaction) // Bao gồm PaymentTransaction để cập nhật
                                            .FirstOrDefaultAsync(t => t.PaymentTransaction.TransactionCode == vnpayResult.OrderId &&
                                                                       t.PaymentTransaction.Gateway == PaymentGateway.VnPay);
            if (appointment == null)
            {
                _logger.LogError("VNPAY Callback: Transaction with OrderId {OrderId} not found in database.", vnpayResult.OrderId);
                throw new KeyNotFoundException($"Transaction {vnpayResult.OrderId} not found.");
            }

            // Kiểm tra xem giao dịch đã được xử lý chưa (tránh xử lý trùng lặp)
            if (appointment.PaymentTransaction.Status != PaymentTransactionStatus.Pending)
            {
                _logger.LogWarning("VNPAY Callback: Transaction {OrderId} already processed with status {Status}.", vnpayResult.OrderId, appointment.Status);
                // Vẫn trả về thành công cho VNPAY nếu giao dịch đã được xử lý thành công
                if (appointment.PaymentTransaction.Status == PaymentTransactionStatus.Success)
                {
                    // Có thể kiểm tra thêm vnpayResult.ResponseCode == "00" để đảm bảo VNPAY cũng báo thành công
                    return; // Không làm gì nữa nếu đã xử lý thành công
                }
                // Nếu trạng thái đã là Failed/Cancelled, có thể cần thêm logic để không ghi đè nếu VNPAY báo thành công sau này
                // Hoặc đơn giản là return để không xử lý lại.
                return;
            }

            // Cập nhật thông tin giao dịch
            appointment.PaymentTransaction.PaymentGatewayTransactionId = vnpayResult.TransactionNo;
            appointment.PaymentTransaction.Message = vnpayResult.Message;
            appointment.PaymentTransaction.ProcessedDate = DateTime.UtcNow;
            appointment.PaymentTransaction.CallbackData = JsonSerializer.Serialize(vnpayResult.RawData);
            appointment.PaymentTransaction.BankCode = vnpayResult.BankCode;
            appointment.PaymentTransaction.PayDate = vnpayResult.PayDate;
            appointment.PaymentTransaction.ResponseCode = vnpayResult.ResponseCode;

            // Cập nhật trạng thái giao dịch và trạng thái thanh toán của Appointment
            if (appointment == null)
            {
                _logger.LogError("VNPAY Callback: Associated Appointment for transaction {OrderId} is null.", vnpayResult.OrderId);
                throw new KeyNotFoundException($"Associated Appointment for transaction {vnpayResult.OrderId} not found.");
            }

            if (vnpayResult.ResponseCode == "00") // Giao dịch VNPAY thành công
            {
                appointment.PaymentTransaction.Status = PaymentTransactionStatus.Success;
                appointment.PaymentStatus = PaymentStatus.Paid;
                appointment.Status = AppointmentStatus.Confirmed; // Xác nhận cuộc hẹn

                _logger.LogInformation("VNPAY Callback: Transaction {OrderId} SUCCESS. Appointment {ApptId} confirmed.", vnpayResult.OrderId, appointment.Id);

                // **ĐIỂM QUAN TRỌNG: Tạo DoctorSchedule NGAY TẠI ĐÂY**
                if (appointment.DoctorId.HasValue && appointment.AppointmentEndDate.HasValue)
                {
                    var dupDoctorSchedule = await _doctorScheduleRepository.GetDuplicatedDoctorScheduleByStartDateEndDateAsync(
                        appointment.DoctorId.Value, appointment.AppointmentStartDate, appointment.AppointmentEndDate);

                    if (dupDoctorSchedule != null && !dupDoctorSchedule.IsAvailable)
                    {
                        // Lỗi này không nên xảy ra nếu bạn đã kiểm tra tính khả dụng của bác sĩ trước khi thanh toán
                        // Nhưng nếu có, bạn phải xử lý:
                        _logger.LogError("VNPAY Callback: Critical Error: Duplicated schedule found after successful payment for Appointment {ApptId}. Doctor {DoctorId} at {Start}-{End}",
                            appointment.Id, appointment.DoctorId, appointment.AppointmentStartDate, appointment.AppointmentEndDate);

                        // Bạn cần quyết định cách xử lý lỗi này:
                        // 1. Hoàn tiền cho bệnh nhân và hủy lịch hẹn (phức tạp hơn)
                        // 2. Để staff can thiệp thủ công
                        // 3. Đánh dấu lịch hẹn là "cần xử lý thủ công" thay vì Confirmed
                        // Hiện tại, tôi sẽ log lỗi và vẫn xác nhận Appointment (giả định đây là lỗi hiếm hoặc có cách giải quyết thủ công).
                        // Tốt nhất là kiểm tra chặt chẽ hơn ở CreateAppointmentAndInitiatePaymentAsync
                    }
                    else
                    {
                        var doctorSchedule = new DoctorSchedule
                        {
                            Id = Guid.NewGuid(),
                            AppointmentId = appointment.Id,
                            DoctorId = appointment.DoctorId.Value,
                            StartTime = appointment.AppointmentStartDate,
                            EndTime = appointment.AppointmentEndDate.Value,
                            IsAvailable = false, // Đánh dấu là đã được đặt
                        };
                        await _doctorScheduleRepository.CreateDoctorScheduleAsync(doctorSchedule);
                        _logger.LogInformation("DoctorSchedule created for Doctor {DoctorId} and Appointment {ApptId} after successful payment.", appointment.DoctorId, appointment.Id);
                    }
                }
                else
                {
                    _logger.LogWarning("DoctorSchedule not created for Appointment {ApptId} due to missing DoctorId or AppointmentEndDate.", appointment.Id);
                }
            }
            else // Giao dịch VNPAY thất bại
            {
                appointment.PaymentTransaction.Status = PaymentTransactionStatus.Failed;
                appointment.PaymentStatus = PaymentStatus.Failed;
                // Nếu thanh toán thất bại, bạn có thể để trạng thái là PendingPayment
                // để người dùng có thể thử thanh toán lại, hoặc chuyển sang Cancelled
                // tùy thuộc vào quy tắc nghiệp vụ của bạn.
                appointment.Status = AppointmentStatus.Cancelled; // Hoặc AppointmentStatus.Cancelled;
                _logger.LogWarning("VNPAY Callback: Transaction {OrderId} FAILED. Appointment {ApptId} payment status set to Failed. Appointment status set to {ApptStatus}.",
                                    vnpayResult.OrderId, appointment.Id, appointment.Status);
            }

            await _appointmentRepository.UpdateAppointmentAsync(appointment); // Cập nhật Appointment với PaymentTransaction

            // Lưu thay đổi vào DB
            try
            {
                await _repository.SaveChangesAsync(); // Lưu transaction và appointment
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes for VNPAY callback for transaction {OrderId}.", vnpayResult.OrderId);
                throw;
            }
        }
    }
}
