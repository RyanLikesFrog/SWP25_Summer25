using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdatePatientTreatmentProtocolStatusRequest
    {
        [Required(ErrorMessage = "ProtocolId là bắt buộc.")]
        public Guid ProtocolId { get; set; }

        [Required(ErrorMessage = "Status là bắt buộc.")]
        public PatientTreatmentStatus Status { get; set; }
    }
}
