using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Patient.Response
{
    public class AppointmentReminderDto
    {
        public Guid AppointmentId { get; set; }
        public string? AppointmentTitle { get; set; }
        public DateTime AppointmentStartDate { get; set; }
        public string? PatientName { get; set; } // Nếu muốn hiển thị tên bệnh nhân
        public string? DoctorName { get; set; }  // Nếu muốn hiển thị tên bác sĩ
        public string? OnlineLink { get; set; }
        public DateTime ReminderSentTime { get; set; } // Thời gian thực tế reminder được gửi
        public string? ReminderMessage { get; set; } // Tin nhắn nhắc nhở cụ thể
    }
}
