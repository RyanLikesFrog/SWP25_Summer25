using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class CreateLabResultRequest
    {
        [Required]
        public Guid? PatientId { get; set; }

        public Guid? TreatmentStageId { get; set; }

        public Guid? DoctorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TestName { get; set; } = string.Empty;

        public string? TestType { get; set; }

        [Required]
        public DateTime TestDate { get; set; }

        [MaxLength(50)]
        public string? ResultSummary { get; set; }

        public string? Conclusion { get; set; }

        public string? Notes { get; set; }
    }
}
