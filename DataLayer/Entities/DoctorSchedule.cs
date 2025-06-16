using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class DoctorSchedule
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Doctor")]
        public Guid DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }


        [ForeignKey("Appointment")]
        public Guid? AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }

        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public bool IsAvailable { get; set; } = true; // update = false sau khi tao PatientTreatmentProtocol
    }
}
