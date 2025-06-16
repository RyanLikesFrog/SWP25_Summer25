using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Patient.Response
{
    public class AppointmentDetailResponse
    {
        public Guid Id { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public string? ApointmentTitle { get; set; }
        public string? PatientFullName { get; set; } // Thêm để hiển thị thông tin bệnh nhân
        public string? DoctorFullName { get; set; } // Thêm để hiển thị thông tin bác sĩ
        public DateTime AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public bool IsAnonymousAppointment { get; set; }
        public string? OnlineLink { get; set; } // Nếu có cuộc hẹn online
        public decimal? Price { get; set; } // Giá của cuộc hẹn
        public PaymentStatus PaymentStatus { get; set; } // Trạng thái thanh toán
        // Quan trọng: Thêm trường này để trả về URL thanh toán cho Front-end
        public string? PaymentRedirectUrl { get; set; }
    }
}

