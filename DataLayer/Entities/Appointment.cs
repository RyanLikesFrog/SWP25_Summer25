using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }
        public virtual Patient? Patient { get; set; }
        public string? ApointmentTitle { get; set; }

        [Required]
        public DateTime AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        [Required]
        public AppointmentType AppointmentType { get; set; } // Sử dụng Enum cho loại cuộc hẹn

        [Required]
        public AppointmentStatus Status { get; set; } // Sử dụng Enum cho trạng thái lịch hẹn

        public string? Notes { get; set; } // TEXT
        public string? OnlineLink { get; set; }

        public bool IsAnonymousAppointment { get; set; }
    }
}
