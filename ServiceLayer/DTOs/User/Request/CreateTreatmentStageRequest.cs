using DataLayer.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class CreateTreatmentStageRequest
    {
        [Required]
        [MaxLength(100)]
        public string StageName { get; set; }

        [Required]
        public int StageNumber { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid PatientTreatmentProtocolId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ReminderTimes { get; set; } // e.g. "08:00,20:00"

        public string? Medicine { get; set; } // e.g. "Paracetamol,Vitamin C"

        [Required]
        public PatientTreatmentStatus Status { get; set; }

        // MEDICAL RECORD FIELDS

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        public string? Diagnosis { get; set; }

        public string? Symptoms { get; set; }

        public string? Prescription { get; set; }

        public string? Notes { get; set; }
    }
}
