using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Enum;

namespace DataLayer.Entities
{
    public class MedicalRecord
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("Doctor")]
        public Guid? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("TreatmentStage")]
        public Guid? TreatmentStageId { get; set; }
        public virtual TreatmentStage? TreatmentStage { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        public string? Diagnosis { get; set; }
        public string? Symptoms { get; set; }
        public string? PrescriptionNote { get; set; }
        public string? Notes { get; set; }

        // Mối quan hệ 1-1: Một MedicalRecord có một Prescription.
        public virtual Prescription? Prescription { get; set; }
    }
}