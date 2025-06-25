using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdateDoctorScheduleRequest
    {
        [Required]
        public Guid Id { get; set; } // ID of the schedule to update

        [Required]
        public Guid DoctorId { get; set; }

        public Guid? AppointmentId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Notes { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
