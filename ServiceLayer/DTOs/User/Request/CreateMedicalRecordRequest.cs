using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class CreateMedicalRecordRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid TreatmentStageId { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        public string? Diagnosis { get; set; }

        public string? Symptoms { get; set; }

        public string? Prescription { get; set; }

        public string? Notes { get; set; }
    }
}
