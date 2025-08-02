using DataLayer.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ServiceLayer.DTOs;

public class CreateTreatmentStageRequest
{
    [Required(ErrorMessage = "Tên giai đoạn không được để trống.")]
    [MaxLength(100, ErrorMessage = "Tên giai đoạn không được vượt quá 100 ký tự.")]
    public string StageName { get; set; }

    [Required(ErrorMessage = "Số thứ tự giai đoạn không được để trống.")]
    public int StageNumber { get; set; }
    public string? Description { get; set; }

    [Required(ErrorMessage = "ID phác đồ điều trị không được để trống.")]
    public Guid PatientTreatmentProtocolId { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ReminderTimes { get; set; }

    [Required(ErrorMessage = "Trạng thái điều trị không được để trống.")]
    public PatientTreatmentStatus Status { get; set; }

    // --- Thông tin Hồ sơ Bệnh án và Đơn thuốc ---

    [Required(ErrorMessage = "Ngày khám bệnh không được để trống.")]
    public DateTime ExaminationDate { get; set; }

    public string? Diagnosis { get; set; }

    public string? Symptoms { get; set; }

    public string? Notes { get; set; }

    // Đơn thuốc được gom vào một đối tượng riêng
    public PrescriptionRequest? Prescription { get; set; }
}

public class PrescriptionRequest
{
    public string? PrescriptionNote { get; set; }

    public List<PrescriptionItemRequest>? PrescriptionItems { get; set; }
}
public class PrescriptionItemRequest
{
    [Required(ErrorMessage = "Tên thuốc không được để trống.")]
    public string DrugName { get; set; }

    [Required(ErrorMessage = "Liều lượng không được để trống.")]
    public string Dosage { get; set; }

    [Required(ErrorMessage = "Tần suất không được để trống")]
    public string Frequency { get; set; }
}