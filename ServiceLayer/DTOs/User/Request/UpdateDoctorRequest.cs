using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdateDoctorRequest
    {
        [Required]
        public Guid DoctorId { get; set; }

        public string? FullName { get; set; }
        public string? Specialization { get; set; }
        public string? Qualifications { get; set; }
        public string? Experience { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureURL { get; set; }
        public bool? IsActive { get; set; }
    }
}
