using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdateMedicalRecord
    {
        // --- Medical Record Fields ---
        [Required]
        public Guid MedicalRecordId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        public Guid? TreatmentStageId { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        public string? Diagnosis { get; set; }
        public string? Symptoms { get; set; }
        public string? PrescriptionNote { get; set; }
        public string? Notes { get; set; }
    }
}
