using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class CreateDoctorScheduleRequest
    {
        [Required]
        public Guid DoctorId { get; set; }

        public Guid? AppointmentId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Notes { get; set; }

        public bool IsAvailable { get; set; } = true; // mặc định là available
    }
}
