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
    public class TreatmentStage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? StageName { get; set; }
        public int StageNumber { get; set; }
        public string? Description { get; set; }

        [ForeignKey("PatientTreatmentProtocol")]
        public Guid? PatientTreatmentProtocolId { get; set; }
        public virtual PatientTreatmentProtocol? PatientTreatmentProtocol { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ReminderTimes { get; set; }
        public PatientTreatmentStatus Status { get; set; }

        public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // Mối quan hệ 1-nhiều với MedicalRecord
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}