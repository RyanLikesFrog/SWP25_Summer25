using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Response
{
    public class CreateUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public UserRole? Role { get; set; }
        public Guid? AssociatedEntityId { get; set; } // DoctorID, PatientID
    }
}
