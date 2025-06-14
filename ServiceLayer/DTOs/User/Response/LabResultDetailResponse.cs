using System;

namespace ServiceLayer.DTOs
{
    public class LabResultDetailResponse
    {
        public Guid Id { get; set; }

        public Guid? PatientId { get; set; }
        public string? PatientName { get; set; }

        public Guid? TreatmentStageId { get; set; }
        public string? TreatmentStageName { get; set; }

        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public string TestName { get; set; } = string.Empty;
        public string? TestType { get; set; }
        public DateTime TestDate { get; set; }

        public string? ResultSummary { get; set; }
        public string? Conclusion { get; set; }
        public string? Notes { get; set; }
    }
}
