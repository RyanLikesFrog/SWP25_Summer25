using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Response
{
    public class NotificationDetailResponse
    {
        public Guid NotificationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? TreatmentStageId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }
    }
}
