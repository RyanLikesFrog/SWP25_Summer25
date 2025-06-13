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
        public int StageNumber { get; set; } // Thứ tự của giai đoạn trong phác đồ
        public string? Description { get; set; }

        [ForeignKey("PatientTreatmentProtocol")]
        public Guid? PatientTreatmentProtocolId { get; set; }
        public virtual PatientTreatmentProtocol? PatientTreatmentProtocol { get; set; } // Navigation property

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Nullable nếu đang điều trị
        public string? ReminderTimes { get; set; } // TEXT (ví dụ: "8:00, 20:00")
        public PatientTreatmentStatus Status { get; set; } // Sử dụng Enum cho trạng thái phác đồ\
        public string? Medicine { get; set; } // Danh sách thuốc, có thể là JSON hoặc chuỗi phân tách bằng dấu phẩy
        public ICollection<LabResult>? LabResults { get; set; }

    }
}
