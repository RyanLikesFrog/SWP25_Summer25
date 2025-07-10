using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class CreateNotificationRequest
    {
        public Guid PatientId { get; set; }

        public Guid? TreatmentStageId { get; set; }

        public Guid? AppointmentId { get; set; }

        public string Message { get; set; } = null!; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
