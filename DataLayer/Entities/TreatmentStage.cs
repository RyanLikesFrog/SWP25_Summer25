using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class TreatmentStage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? StageName { get; set; }

        public string? Description { get; set; }

        [ForeignKey("PatientTreatmentProtocol")]
        public Guid? PatientTreatmentProtocolId { get; set; }
        public virtual PatientTreatmentProtocol? PatientTreatmentProtocol { get; set; } // Navigation property

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Nullable nếu đang điều trị

        [MaxLength(50)]
        public string? ReminderFrequency { get; set; }
        public string? ReminderTimes { get; set; } // TEXT (ví dụ: "8:00, 20:00")
        public string? CustomProtocolDetails { get; set; } // Lưu trữ JSON hoặc TEXT
        public ICollection<LabResult>? LabResults { get; set; }

    }
}
