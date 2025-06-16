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
        private readonly IRepository _repository;
        private readonly VnPayPaymentClient _vnPayClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IPatientRepository patientRepository,
            IUserRepository userRepository,
            IAppointmentRepository appointmentRepository,
            IRepository repository,
            IDoctorScheduleRepository doctorScheduleRepository,
            IDoctorRepository doctorRepository,
            VnPayPaymentClient vnPayClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AppointmentService> logger)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _repository = repository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _doctorRepository = doctorRepository;
            _vnPayClient = vnPayClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<List<Appointment>>? GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAppointmentsAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
        }

        //public async Task<AppointmentDetailResponse> RegisterAppointmentAsync(UserCreateAppointmentRequest request)
        //{
        //    // Sử dụng các Repository để lấy dữ liệu
        //    var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
        //    if (patient == null)
        //    {
        //        throw new ArgumentException("Patient not found with the provided Patient ID.");
        //    }

        //    if (request.DoctorId.HasValue)
        //    {
        //        var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId.Value);
        //        if(doctor == null)
        //        {
        //            throw new ArgumentException("Doctor not found with the provided Doctor ID");
        //        }
        //    }

        //    // Tạo Entity
        //    var newAppointment = new Appointment
        //    {
        //        Id = Guid.NewGuid(),
        //        PatientId = request.PatientId,
        //        DoctorId = request.DoctorId, // Có thể null nếu không có bác sĩ
        //        AppointmentTitle = request.ApointmentTitle,
        //        AppointmentStartDate = request.AppointmentStartDate,
        //        AppointmentType = request.AppointmentType,
        //        Status = AppointmentStatus.Pending,
        //        Notes = request.Notes,
        //        IsAnonymousAppointment = request.IsAnonymousAppointment,
        //    };
        //    await _appointmentRepository.CreateAppointmentAsync(newAppointment);
        //    await _repository.SaveChangesAsync();
        //    // Map Entity sang DTO
        //    return new AppointmentDetailResponse
        //    {
        //        Id = newAppointment.Id,
        //        PatientId = newAppointment.PatientId.Value,
        //        PatientFullName = patient.FullName,
        //        AppointmentStartDate = newAppointment.AppointmentStartDate,
        //        AppointmentType = newAppointment.AppointmentType,
        //        Status = newAppointment.Status,
        //        Notes = newAppointment.Notes,
        //        IsAnonymousAppointment = newAppointment.IsAnonymousAppointment,
        //        ApointmentTitle = newAppointment.AppointmentTitle
        //    };
        //}

        //public async Task<AppointmentDetailResponse> StaffUpdateAppointmentAsync(StaffManageAppointmentRequest request)
        //{
        //    var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
        //    if (appointment == null)
        //    {
        //        throw new ArgumentException("Appointment not found.");
        //    }
        //    // Cập nhật thông tin cuộc hẹn
        //    appointment.Status = request.NewStatus;
        //    appointment.AppointmentType = request.AppointmentType;
        //    appointment.DoctorId = request.DoctorId;
        //    appointment.AppointmentStartDate = request.AppointmentStartDate;
        //    appointment.AppointmentEndDate = request.AppointmentEndDate;
        //    appointment.AppointmentTitle = request.AppointmentTitle;
        //    appointment.Notes = request.Notes;
        //    appointment.OnlineLink = request.OnlineLink;
        //    // Lưu thay đổi
        //    await _appointmentRepository.UpdateAppointmentAsync(appointment);

        //    // Nếu có thay đổi về bác sĩ, cần cập nhật schedule cho bác sĩ
        //    var dupDoctorSchedule = await _doctorScheduleRepository.GetDuplicatedDoctorScheduleByStartDateEndDateAsync(appointment.DoctorId, appointment.AppointmentStartDate, appointment.AppointmentEndDate);
        //    if (dupDoctorSchedule != null)
        //    {
        //        throw new ArgumentException("Duplicated schedule");
        //    }
        //    else
        //    {
        //        var doctorSchedule = new DoctorSchedule
        //        {
        //            Id = Guid.NewGuid(),
        //            AppointmentId = appointment.Id,
        //            DoctorId = appointment.DoctorId.Value,
        //            StartTime = request.AppointmentStartDate,
        //            EndTime = request.AppointmentEndDate.Value,
        //            IsAvailable = true,
        //        };
        //        await _doctorScheduleRepository.CreateDoctorScheduleAsync(doctorSchedule);
        //    }

        //    await _repository.SaveChangesAsync();
        //    return new AppointmentDetailResponse
        //    {
        //        Id = appointment.Id,
        //        PatientId = appointment.PatientId.Value,
        //        DoctorId = appointment.DoctorId,
        //        PatientFullName = appointment.Patient?.FullName, // Lấy tên bệnh nhân nếu có
        //        //DoctorFullName = appointment.Doctor?.FullName, // Lấy tên bác sĩ nếu có
        //        AppointmentStartDate = appointment.AppointmentStartDate,
        //        AppointmentEndDate = appointment.AppointmentEndDate,
        //        AppointmentType = appointment.AppointmentType,
        //        Status = appointment.Status,
        //        Notes = appointment.Notes,
        //        IsAnonymousAppointment = appointment.IsAnonymousAppointment,
        //        OnlineLink = appointment.OnlineLink,
        //        ApointmentTitle = appointment.AppointmentTitle
        //    };
        //}

        public async Task<(List<Appointment?> appointments, string Message)> ViewAppointmentAsync(Guid doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);

            if (appointments == null || !appointments.Any())
            {
                return (null, "Khong tim thay lich hen.");
            }

            return (appointments, "Tim lich hen thanh cong.");
        }

        public async Task<AppointmentDetailResponse> CreateAppointmentAndInitiatePaymentAsync(UserCreateAppointmentRequest request)
        {
            var patient = await _userRepository.GetUserByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new ArgumentException("Patient not found with the provided Patient ID.");
            }

            if (request.DoctorId.HasValue)
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId.Value); // Dùng FindAsync nếu _context trực tiếp
                if (doctor == null)
                {
                    throw new ArgumentException("Doctor not found with the provided Doctor ID.");
                }

                // Kiểm tra trùng lịch của bác sĩ NẾU LỊCH ĐÃ ĐƯỢC ĐẶT (IsAvailable=false)
                // (Không cần kiểm tra nếu DoctorSchedule chỉ tạo sau thanh toán thành công)
                // Tuy nhiên, nếu bạn muốn đảm bảo bác sĩ KHÔNG CÓ LỊCH BẬN tại thời điểm này, hãy kiểm tra
                var existingDoctorSchedule = await _doctorScheduleRepository
                    .GetDuplicatedDoctorScheduleByStartDateEndDateAsync(request.DoctorId.Value, request.AppointmentStartDate, request.AppointmentEndDate);

                if (existingDoctorSchedule != null && !existingDoctorSchedule.IsAvailable) // Nếu đã có lịch bận
                {
                    throw new ArgumentException("The selected doctor is not available at the specified time.");
                }
            }
            else
            {
                throw new ArgumentException("Doctor must be selected for the appointment.");
            }


            // Bước 2: Tạo PaymentTransaction
            var newTransaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Amount = 200000,
                Currency = "VND",
                Gateway = PaymentGateway.VnPay, // Luôn là VNPAY
                Status = PaymentTransactionStatus.Pending,
                TransactionCode = Guid.NewGuid().ToString("N"), // Mã tham chiếu giao dịch nội bộ
                CreatedDate = DateTime.UtcNow
            };

            // Bước 3: Tạo Appointment
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
                Status = AppointmentStatus.Pending, // Trạng thái chờ thanh toán
                Price = 200000,
                PaymentStatus = PaymentStatus.Pending, // Trạng thái thanh toán ban đầu
                PaymentTransactionId = newTransaction.Id, // Liên kết với giao dịch
                PaymentTransaction = newTransaction // Gán navigation property
            };

            // Bước 4: Khởi tạo yêu cầu thanh toán VNPAY
            var clientIpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var vnPayRequest = new VnPayCreatePaymentRequest
            {
                Amount = (long)200000 * 100, // VNPAY yêu cầu số tiền * 100
                OrderId = newTransaction.TransactionCode,
                OrderInfo = $"Thanh toan lich hen {appointment.Id}",
                ReturnUrl = _vnPayClient.GetReturnUrl(),
                Locale = "vn"
            };

            string paymentRedirectUrl = _vnPayClient.CreatePaymentUrl(vnPayRequest, clientIpAddress);

            if (string.IsNullOrEmpty(paymentRedirectUrl))
            {
                _logger.LogError("Failed to create VNPAY payment URL for appointment {ApptId}.", appointment.Id);
                // Nếu không tạo được URL, coi như giao dịch thất bại ngay lập tức
                newTransaction.Status = PaymentTransactionStatus.Failed;
                appointment.PaymentStatus = PaymentStatus.Failed;
                appointment.Status = AppointmentStatus.Cancelled; // Hoặc bạn có thể giữ Pending để người dùng thử lại
                await _appointmentRepository.CreateAppointmentAsync(appointment); // Vẫn lưu để có record thất bại
                await _repository.SaveChangesAsync();
                throw new Exception("Could not initiate VNPAY payment. Please try again.");
            }

            // Cập nhật PaymentUrl vào transaction
            newTransaction.PaymentUrl = paymentRedirectUrl;

            // Bước 5: Lưu Appointment và PaymentTransaction vào Database
            // Cả hai nên được lưu trong cùng một transaction để đảm bảo tính nhất quán
            await _appointmentRepository.CreateAppointmentAsync(appointment); // Thêm Appointment
                                                                              // _context.PaymentTransactions.Add(newTransaction); // PaymentTransaction đã được thêm qua navigation property của Appointment

            await _repository.SaveChangesAsync(); // Lưu cả hai cùng lúc

            _logger.LogInformation("Appointment {ApptId} and PaymentTransaction {TransId} created. Redirecting to VNPAY: {Url}",
                                    appointment.Id, newTransaction.Id, paymentRedirectUrl);

            // Bước 6: Trả về DTO chứa PaymentRedirectUrl cho Front-end
            return new AppointmentDetailResponse
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentStartDate = appointment.AppointmentStartDate,
                AppointmentEndDate = appointment.AppointmentEndDate,
                AppointmentType = appointment.AppointmentType, // Ép kiểu nếu cần
                ApointmentTitle = appointment.AppointmentTitle,
                Status = appointment.Status, // Chuyển enum sang string
                Price = appointment.Price,
                PaymentStatus = appointment.PaymentStatus, // Chuyển enum sang string
                OnlineLink = appointment.OnlineLink,
                IsAnonymousAppointment = appointment.IsAnonymousAppointment,
                PaymentRedirectUrl = paymentRedirectUrl
            };
        }
    }
}
