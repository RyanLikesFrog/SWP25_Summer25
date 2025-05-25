using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")] // Định nghĩa khóa ngoại tới bảng User
        public Guid UserId { get; set; }
        public virtual User User { get; set; } // Navigation property

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; } // Sử dụng Enum cho giới tính

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? ContactPersonName { get; set; }

        [MaxLength(20)]
        public string? ContactPersonPhone { get; set; }

        public bool IsAnonymous { get; set; } // Cờ ẩn danh

        // Navigation properties cho mối quan hệ 1-N
        public virtual ICollection<Appointment>? Appointments { get; set; }
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public virtual ICollection<LabResult>? LabResults { get; set; }
        public virtual ICollection<PatientTreatmentProtocol>? PatientTreatmentProtocols { get; set; }
    }
}
