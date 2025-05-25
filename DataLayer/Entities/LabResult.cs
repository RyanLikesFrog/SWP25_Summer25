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

        [Required]
        [MaxLength(100)]
        public string? TestType { get; set; }

        [Required]
        public DateTime TestDate { get; set; }

        [MaxLength(50)]
        public string? ResultValue { get; set; }

        [MaxLength(20)]
        public string? Unit { get; set; }

        [MaxLength(100)]
        public string? ReferenceRange { get; set; }

        public string? Notes { get; set; } // TEXT
    }
}
