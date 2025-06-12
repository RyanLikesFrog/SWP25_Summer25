using DataLayer.Enum;
using System;
using System.Collections.Generic;

namespace ServiceLayer.DTOs
{
    public class PatientTreatmentProtocolDetailResponse
    {
        public Guid Id { get; set; }

        public Guid? PatientId { get; set; }

        public Guid? DoctorId { get; set; }

        public Guid? ProtocolId { get; set; }

        public Guid? AppointmentId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public PatientTreatmentStatus Status { get; set; }

        public List<TreatmentStageResponse>? TreatmentStages { get; set; }
    }
}
