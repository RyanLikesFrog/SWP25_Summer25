using DataLayer.Entities;
using DataLayer.Enum;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.DTOs.User.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId);
        public Task<List<Appointment>>? GetAllAppointmentsAsync();
        //Task<AppointmentDetailResponse> RegisterAppointmentAsync(UserCreateAppointmentRequest request);
        //Task<AppointmentDetailResponse> StaffUpdateAppointmentAsync(StaffManageAppointmentRequest request);
        public Task<List<Appointment>>? GetPaidAppointmentsAsync();
        public Task<(List<Appointment?> appointments, string Message)> ViewAppointmentAsync(Guid doctorId);
        public Task<AppointmentDetailResponse> CreateAppointmentAndInitiatePaymentAsyncV2(UserCreateAppointmentRequest request);
        public Task<Appointment?> UpdateAppointmentAsync(UpdateAppointmentRequest request);
        public Task<Appointment?> ReArrangeDateAppointmentAsync(UpdateReArrangeDateAppointmentRequest request);
        public Task UpdatePaymentStatusAsync(
            string orderId, // Đây là TransactionCode nội bộ của bạn
            PaymentTransactionStatus transactionStatus,
            PaymentStatus appointmentPaymentStatus,
            int? momoResultCode,
            string? momoMessage,
            string? momoTransactionId,
            string? momoRequestId,
            string? momoSignature,
            DateTime? momoResponseTime,
            string? momoExtraData);
    }
}
