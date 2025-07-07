using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdateReArrangeDateAppointmentRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime NewAppointmentStartDate { get; set; } 

        public DateTime? NewAppointmentEndDate { get; set; } 
    }
}
