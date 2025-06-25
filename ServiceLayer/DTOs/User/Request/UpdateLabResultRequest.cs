using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs.LabResult.Request
{
    public class UpdateLabResultRequest
    {
        [Required]
        public Guid LabResultId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        public Guid? TreatmentStageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? TestName { get; set; }

        public string? TestType { get; set; }

        [Required]
        public DateTime TestDate { get; set; }

        [MaxLength(50)]
        public string? ResultSummary { get; set; }

        public string? Conclusion { get; set; }

        public string? Notes { get; set; }

        // --- Optional: Single image ID reference (legacy)
        public Guid? LabPictureId { get; set; }

        // --- Upload new pictures (with metadata)
        public List<IFormFile>? LabResultPictures { get; set; }

        public List<string>? LabPictureNames { get; set; }

        public List<bool>? LabPictureIsActiveFlags { get; set; }
    }
}
