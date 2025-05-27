using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UserDto
    {
        public Guid UserID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DoctorDto? DoctorInfo { get; set; } // Nếu người dùng là bác sĩ
        public PatientDto? PatientInfo { get; set; } // Nếu người dùng là bệnh nhân
    }
}
