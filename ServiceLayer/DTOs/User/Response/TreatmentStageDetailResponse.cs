using System;
using System.Collections.Generic;
using DataLayer.Enum;

namespace ServiceLayer.DTOs
{
    public class TreatmentStageDetailResponse
    {
        public Guid Id { get; set; }

        public string? StageName { get; set; }

        public int StageNumber { get; set; }

        public string? Description { get; set; }

        public Guid? PatientTreatmentProtocolId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ReminderTimes { get; set; }

        public PatientTreatmentStatus Status { get; set; }

        public string? Medicine { get; set; }

        public List<PrescriptionItemDto>? PrescriptionItems { get; set; }

        public List<LabResultDto>? LabResults { get; set; }

        public MedicalRecordDto? MedicalRecord { get; set; }
    }

    public class PrescriptionItemDto
    {
        public string DrugName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
    }

    public class LabResultDto
    {
        public Guid Id { get; set; }
        public string? ResultType { get; set; }
        public string? Value { get; set; }
        public DateTime? Date { get; set; }
    }

    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? TreatmentStageId { get; set; }
        public DateTime ExaminationDate { get; set; }
        public string? Diagnosis { get; set; }
        public string? Symptoms { get; set; }
        public string? PrescriptionNote { get; set; }
        public string? Notes { get; set; }
    }
}
