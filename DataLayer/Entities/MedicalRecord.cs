using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string? Diagnosis { get; set; } // TEXT
        public string? Symptoms { get; set; } // TEXT
        public string? Prescription { get; set; } // TEXT
        public string? Notes { get; set; } // TEXT
    }
}
