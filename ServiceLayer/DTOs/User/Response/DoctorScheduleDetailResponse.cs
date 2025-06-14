using System;

namespace ServiceLayer.DTOs
{
    public class DoctorScheduleDetailResponse
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }

        public Guid? AppointmentId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? Notes { get; set; }

        public bool IsAvailable { get; set; }
    }
}
