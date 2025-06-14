using DataLayer.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class TreatmentStageResponse
    {
        public Guid Id { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? ReminderFrequency { get; set; }
        public string? ReminderTimes { get; set; }
        public string? CustomProtocolDetails { get; set; }
        public int LabResultCount { get; set; }
        public PatientTreatmentStatus Status { get; set; }
    }
}
