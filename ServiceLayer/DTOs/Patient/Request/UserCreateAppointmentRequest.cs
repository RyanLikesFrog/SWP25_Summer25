using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Patient.Request
{
    public class UserCreateAppointmentRequest
    {
        [Required(ErrorMessage = "userId is required.")]
        public Guid UserId { get; set; }

        // DoctorID có thể là tùy chọn nếu bệnh nhân không yêu cầu bác sĩ cụ thể
        public Guid? DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment start date and time are required.")]
        public DateTime AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; } // Có thể để trống nếu không có thời gian kết thúc cụ thể

        [Required(ErrorMessage = "Appointment type is required.")]
        [EnumDataType(typeof(AppointmentType), ErrorMessage = "Invalid Appointment Type.")]
        public AppointmentType AppointmentType { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        public bool IsAnonymousAppointment { get; set; } = false; // Mặc định là không ẩn danh

        public string? ApointmentTitle { get; set; }
    }
}
