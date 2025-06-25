using DataLayer.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class CreatePatientTreatmentProtocolRequest
    {
        internal Guid patientTreatmentProtocolId;

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        public Guid? ARVProtocolId { get; set; } // Optional: null if it's a custom protocol

        public Guid? AppointmentId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public PatientTreatmentStatus Status { get; set; }
    }
}
