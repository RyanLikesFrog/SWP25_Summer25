using DataLayer.Entities;
using System;

namespace ServiceLayer.DTOs
{
    public class LabResultDetailResponse
    {
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? TreatmentStageId { get; set; }
    public string? TestName { get; set; }
    public string? TestType { get; set; }
    public DateTime TestDate { get; set; }
    public string? ResultSummary { get; set; }
    public string? Conclusion { get; set; }
    public string? Notes { get; set; }
    public List<LabPicture> LabPictures { get; set; } = new();

    }
}
