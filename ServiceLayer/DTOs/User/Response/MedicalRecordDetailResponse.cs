using System;

namespace ServiceLayer.DTOs
{
    public class MedicalRecordDetailResponse
    {
        public Guid Id { get; set; }

        public Guid? PatientId { get; set; }
        public string? PatientName { get; set; }

        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public Guid? TreatmentStageId { get; set; }
        public string? TreatmentStageName { get; set; }

        public DateTime ExaminationDate { get; set; }

        public string? Diagnosis { get; set; }

        public string? Symptoms { get; set; }

        public string? Prescription { get; set; }

        public string? Notes { get; set; }
    }
}
