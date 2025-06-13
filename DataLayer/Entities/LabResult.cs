using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class LabResult
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("TreatmentStage")]
        public Guid? TreatmentStageId { get; set; }
        public virtual TreatmentStage? TreatmentStage { get; set; }
        [ForeignKey("Doctor")]
        public Guid? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; } // Navigation property

        [Required]
        [MaxLength(100)]
        public string? TestName { get; set; }
        public string? TestType { get; set; } // Loại xét nghiệm
        [Required]
        public DateTime TestDate { get; set; }

        [MaxLength(50)]
        public string? ResultSummary { get; set; }  
        public string? Conclusion { get; set; } // Kết luận từ bác sĩ
        public string? Notes { get; set; } // TEXT
    }
}
