using DataLayer.Entities;
using DataLayer.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.DTOs.Payment;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
using ServiceLayer.PaymentGateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IRepository _repository;
        private readonly ILogger<AppointmentService> _logger;
        private readonly IMomoClient _momoClient;

        public AppointmentService(
            IPatientRepository patientRepository,
            IUserRepository userRepository,
            IAppointmentRepository appointmentRepository,
            IRepository repository,
            IDoctorScheduleRepository doctorScheduleRepository,
            IPaymentTransactionRepository paymentTransactionRepository,
            IDoctorRepository doctorRepository,
            ILogger<AppointmentService> logger,
            IMomoClient momoClient)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _repository = repository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _doctorRepository = doctorRepository;
            _logger = logger;
            _momoClient = momoClient;
        }
        public async Task<List<Appointment>>? GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAppointmentsAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<AppointmentDetailResponse> CreateAppointmentAndInitiatePaymentAsyncV2(UserCreateAppointmentRequest request)
        {

            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId.Value);
            if (patient == null)
            {
                throw new ArgumentException("Patient not found with the provided Patient ID.");
            }

            if (request.DoctorId.HasValue)
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId.Value);
                if (doctor == null)
                {
                    throw new ArgumentException("Doctor not found with the provided Doctor ID.");
                }

                var existingDoctorSchedule = await _doctorScheduleRepository
                    .GetDuplicatedDoctorScheduleByStartDateEndDateAsync(request.DoctorId.Value, request.AppointmentStartDate, request.AppointmentEndDate);

                if (existingDoctorSchedule != null && !existingDoctorSchedule.IsAvailable)
                {
                    throw new ArgumentException("The selected doctor is not available at the specified time.");
                }
            }
            else
            {
                throw new ArgumentException("Doctor must be selected for the appointment.");
            }

            // Bước 1: Tạo Appointment trước để có ID
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                AppointmentStartDate = request.AppointmentStartDate,
                AppointmentEndDate = request.AppointmentEndDate,
                AppointmentType = request.AppointmentType,
                AppointmentTitle = request.ApointmentTitle,
                Notes = request.Notes,
                IsAnonymousAppointment = request.IsAnonymousAppointment,
                Status = AppointmentStatus.Pending,
                Price = 200000,
                PaymentStatus = PaymentStatus.Pending,
            };

            // Bước 2: Tạo PaymentTransaction
            var newTransaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Amount = 200000,
                Currency = "VND",
                Status = PaymentTransactionStatus.Pending,
                TransactionCode = Guid.NewGuid().ToString("N"), // Mã tham chiếu nội bộ, sẽ dùng làm OrderId cho Momo
                CreatedDate = DateTime.UtcNow,
                AppointmentId = appointment.Id // Gán AppointmentId ngay lập tức
            };

            // Gán PaymentTransactionId cho Appointment
            appointment.PaymentTransactionId = newTransaction.Id;
            appointment.PaymentTransaction = newTransaction; // Gán navigation property

            // Bước 3: Khởi tạo yêu cầu thanh toán MOMO
            var momoRequest = new MomoCreatePaymentRequest
            {
                Amount = 200000,
                OrderId = newTransaction.TransactionCode, // Sử dụng TransactionCode làm OrderId cho Momo
                OrderInfo = $"Thanh toan lich hen {appointment.Id}",
                ExtraData = ""
            };

            MomoCreatePaymentResponse momoResponse = await _momoClient.CreatePaymentAsync(momoRequest);

            if (momoResponse == null || momoResponse.ResultCode != 0 || string.IsNullOrEmpty(momoResponse.PayUrl))
            {
                _logger.LogError("Failed to create MOMO payment. ResultCode: {ResultCode}, Message: {Message}",
                                momoResponse?.ResultCode, momoResponse?.Message);

                // Cập nhật các trường Momo và trạng thái khi thất bại ban đầu
                newTransaction.MomoRequestId = momoResponse?.RequestId;
                newTransaction.MomoOrderId = momoResponse?.OrderId;
                //fix
                newTransaction.MomoResultCode = momoResponse?.ResultCode.ToString();
                newTransaction.MomoMessage = momoResponse?.Message;
                newTransaction.MomoResponseTime = momoResponse != null && momoResponse.ResponseTime != 0 ? DateTimeOffset.FromUnixTimeMilliseconds(momoResponse.ResponseTime).UtcDateTime : (DateTime?)null;
                newTransaction.MomoExtraData = momoResponse?.ExtraData;
                newTransaction.Status = PaymentTransactionStatus.Failed;
                newTransaction.Message = momoResponse?.Message ?? "Momo payment initiation failed";
                newTransaction.ProcessedDate = DateTime.UtcNow;
                appointment.PaymentStatus = PaymentStatus.Failed;
                appointment.Status = AppointmentStatus.Cancelled;

                await _paymentTransactionRepository.AddPaymentTransactionAsync(newTransaction);
                await _appointmentRepository.CreateAppointmentAsync(appointment);
                await _repository.SaveChangesAsync();
                throw new Exception($"Could not initiate MOMO payment: {momoResponse?.Message ?? "Unknown error"}. Please try again.");
            }

            // Cập nhật các trường Momo và PaymentUrl khi khởi tạo thành công
            newTransaction.PaymentUrl = momoResponse.PayUrl;
            newTransaction.MomoRequestId = momoResponse.RequestId;
            newTransaction.MomoOrderId = momoResponse.OrderId;
            newTransaction.MomoResultCode = momoResponse.ResultCode.ToString();
            newTransaction.MomoMessage = momoResponse.Message;
            newTransaction.MomoResponseTime = momoResponse.ResponseTime != 0 ? DateTimeOffset.FromUnixTimeMilliseconds(momoResponse.ResponseTime).UtcDateTime : (DateTime?)null;
            newTransaction.MomoExtraData = momoResponse.ExtraData;
            newTransaction.Message = "Momo payment initiated successfully, awaiting user payment.";


            // Bước 4: Lưu Appointment và PaymentTransaction vào Database
            await _paymentTransactionRepository.AddPaymentTransactionAsync(newTransaction);
            await _appointmentRepository.CreateAppointmentAsync(appointment);

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Appointment {ApptId} and PaymentTransaction {TransId} created. Redirecting to MOMO: {Url}",
                                    appointment.Id, newTransaction.Id, momoResponse.PayUrl);

            // Bước 5: Trả về DTO chứa PaymentRedirectUrl cho Front-end
            return new AppointmentDetailResponse
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentStartDate = appointment.AppointmentStartDate,
                AppointmentEndDate = appointment.AppointmentEndDate,
                AppointmentType = appointment.AppointmentType,
                ApointmentTitle = appointment.AppointmentTitle,
                Status = appointment.Status,
                Price = appointment.Price,
                PaymentStatus = appointment.PaymentStatus,
                OnlineLink = appointment.OnlineLink,
                IsAnonymousAppointment = appointment.IsAnonymousAppointment,
                PaymentRedirectUrl = momoResponse.PayUrl,
                QrCodeImageUrl = momoResponse.QrCodeUrl, // Nếu có
            };
        }

        // Phương thức để cập nhật trạng thái thanh toán từ Callback của Momo
        public async Task UpdatePaymentStatusAsync(
            string orderId, // Đây là TransactionCode nội bộ của bạn
            PaymentTransactionStatus transactionStatus,
            PaymentStatus appointmentPaymentStatus,
            int? momoResultCode,
            string? momoMessage,
            string? momoTransactionId,
            string? momoRequestId,
            string? momoSignature,
            DateTime? momoResponseTime,
            string? momoExtraData)
        {
            // Lấy PaymentTransaction dựa trên TransactionCode (OrderId của Momo)
            var transaction = await _paymentTransactionRepository.GetPaymentTransactionByTransactionCodeAsync(orderId);
            if (transaction == null)
            {
                _logger.LogWarning("Payment transaction with code {OrderId} not found for status update.", orderId);
                return;
            }

            // Lấy Appointment liên quan thông qua AppointmentId trong PaymentTransaction
            // (Hoặc bạn có thể include Appointment khi lấy transaction nếu repository hỗ trợ)
            if (transaction.Appointment == null)
            {
                _logger.LogError("Associated appointment for transaction {TransId} (AppointmentId: {ApptId}) not found.", transaction.Id, transaction.AppointmentId);
                return;
            }

            // Cập nhật các trường chính của PaymentTransaction
            transaction.Status = transactionStatus;
            transaction.Message = momoMessage;
            transaction.ProcessedDate = DateTime.UtcNow;

            // Cập nhật các trường Momo-specific
            transaction.MomoResultCode = momoResultCode.ToString();
            transaction.MomoMessage = momoMessage;
            transaction.MomoTransactionId = momoTransactionId;
            transaction.MomoRequestId = momoRequestId;
            transaction.MomoSignature = momoSignature;
            transaction.MomoResponseTime = momoResponseTime;
            transaction.MomoExtraData = momoExtraData;

            // Cập nhật trạng thái của Appointment
            transaction.Appointment.PaymentStatus = appointmentPaymentStatus;
            transaction.Appointment.Status = (appointmentPaymentStatus == PaymentStatus.Paid) ? AppointmentStatus.Confirmed : transaction.Appointment.Status

            await _repository.SaveChangesAsync();
            _logger.LogInformation("Payment transaction {TransId} and related appointment {ApptId} updated to {Status}", orderId, transaction.AppointmentId, transactionStatus);
        }

        public async Task<(List<Appointment?> appointments, string Message)> ViewAppointmentAsync(Guid doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);

            if (appointments == null || !appointments.Any())
            {
                return (null, "Khong tim thay lich hen.");
            }

            return (appointments, "Tim lich hen thanh cong.");
        }

        public async Task<List<Appointment>>? GetPaidAppointmentsAsync()
        {
            var appointments = await GetAllAppointmentsAsync();
            return appointments
                .Where(a => a.PaymentStatus == PaymentStatus.Paid)
                .ToList();
        }

        public async Task<Appointment?> UpdateAppointmentAsync(UpdateAppointmentRequest request)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);

            if (appointment == null)
            {
                throw new ArgumentException($"Không tìm thấy Appointment với ID {request.AppointmentId}.");
            }

            appointment.OnlineLink = request.OnlineLink;

            await _appointmentRepository.UpdateAppointmentAsync(appointment);
            await _repository.SaveChangesAsync();

            return appointment;
        }

        public async Task<Appointment> ReArrangeDateAppointmentAsync(UpdateReArrangeDateAppointmentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Yêu cầu dời lịch không được để trống.");
            }

            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(request.Id);

            if (existingAppointment == null)
            {
                throw new ArgumentException($"Không tìm thấy lịch hẹn với ID {request.Id}.");
            }

            if (request.NewAppointmentStartDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Không thể dời lịch hẹn sang thời gian đã qua.");
            }

            if (request.NewAppointmentEndDate.HasValue && request.NewAppointmentEndDate < request.NewAppointmentStartDate)
            {
                throw new InvalidOperationException("Ngày kết thúc cuộc hẹn không thể trước ngày bắt đầu.");
            }

            bool isTimeSlotAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                existingAppointment.DoctorId,
                request.NewAppointmentStartDate,
                request.NewAppointmentEndDate,
                request.Id
            );

            if (!isTimeSlotAvailable)
            {
                throw new InvalidOperationException("Khung thời gian đã chọn không khả dụng hoặc trùng với lịch hẹn khác.");
            }

            existingAppointment.AppointmentStartDate = request.NewAppointmentStartDate;
            existingAppointment.AppointmentEndDate = request.NewAppointmentEndDate;

            existingAppointment.Status = AppointmentStatus.ReArranged;

            try
            {
                return await _appointmentRepository.ReArrangeDateAppointmentAsync(existingAppointment);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Đã xảy ra lỗi khi dời lịch hẹn với ID {request.Id}.", ex);
            }
        }

        public async Task<bool> UpdateAppointmentStatusAsync(UpdateAppointmentStatusRequest request)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null) return false;

            await _appointmentRepository.UpdateStatusAsync(appointment, request.NewStatus, request.Note);
            return true;
        }
    }
}
