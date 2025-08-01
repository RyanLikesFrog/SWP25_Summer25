using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    public class Prescription
    {
        [Key]
        public Guid Id { get; set; }

        // Mối quan hệ 1-1 với MedicalRecord
        [Required]
        [ForeignKey("MedicalRecord")]
        public Guid MedicalRecordId { get; set; }
        public virtual MedicalRecord? MedicalRecord { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }

        // Mối quan hệ 1-nhiều với PrescriptionItem.
        public virtual ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }
}