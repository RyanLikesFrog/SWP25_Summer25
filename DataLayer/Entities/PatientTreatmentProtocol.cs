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
    public class PatientTreatmentProtocol
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("Doctor")]
        public Guid? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("ARVProtocol")]
        public Guid? ProtocolId { get; set; } // Nullable nếu là phác đồ tùy chỉnh hoàn toàn
        public virtual ARVProtocol? ARVProtocol { get; set; }

        [ForeignKey("Appointment")]
        public Guid? AppointmentId {get; set; }
        public virtual Appointment? Appointment { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public PatientTreatmentStatus Status { get; set; } // Sử dụng Enum cho trạng thái phác đồ\
        public virtual ICollection<TreatmentStage>? TreatmentStages { get; set; }
    }
}
