using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdateTreatmentStateMedicineRequest
    {
        [Required(ErrorMessage = "Treatment Stage ID is required.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Medicine information is required.")]
        public string? Medicine { get; set; }
    }
}
