using DataLayer.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class CreatePatientTreatmentProtocolRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        public Guid? ProtocolId { get; set; } // Optional: null if it's a custom protocol

        public Guid? AppointmentId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public PatientTreatmentStatus Status { get; set; }
    }
}
