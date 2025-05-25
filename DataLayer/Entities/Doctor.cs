using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(100)]
        public string? Specialization { get; set; }

        public string? Qualifications { get; set; } // TEXT
        public string? Experience { get; set; } // TEXT
        public string? Bio { get; set; } // TEXT

        [MaxLength(255)]
        public string? ProfilePictureURL { get; set; }

        // Navigation properties cho mối quan hệ 1-N
        public virtual ICollection<DoctorSchedule>? DoctorSchedules { get; set; }
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public virtual ICollection<PatientTreatmentProtocol>? PatientTreatmentProtocols { get; set; }
    }
}
