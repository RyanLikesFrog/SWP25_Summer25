using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdateAppointmentRequest
    {
        [Required]
        public Guid AppointmentId { get; set; }

        [Required]
        public string? OnlineLink { get; set; }
    }
}
