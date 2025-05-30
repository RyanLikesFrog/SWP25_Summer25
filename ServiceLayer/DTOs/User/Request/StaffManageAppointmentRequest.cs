using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class StaffManageAppointmentRequest
    {
        [Required(ErrorMessage = "Appointment ID is required.")]
        public Guid AppointmentId { get; set; }
        public AppointmentStatus NewStatus { get; set; }
        public AppointmentType AppointmentType { get; set; } // Sử dụng Enum cho loại cuộc hẹn

        // DoctorId là tùy chọn, chỉ cần khi gán bác sĩ
        public Guid? DoctorId { get; set; }
        public DateTime AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }


        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? AppointmentTitle { get; set; }
        public string? Notes { get; set; } // Ghi chú của nhân viên
        public string? OnlineLink { get; set; } // Nếu có cuộc hẹn online
    }
}
