using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? TreatmentStageId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }

        //Navigation properties
        public virtual Patient? Patient { get; set; }
        public virtual TreatmentStage? TreatmentStage { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}
