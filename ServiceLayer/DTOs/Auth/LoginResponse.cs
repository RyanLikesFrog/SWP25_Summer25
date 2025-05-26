using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Các ID tùy thuộc vào vai trò
        public Guid? PatientId { get; set; } // Chỉ có giá trị nếu Role là Patient
        public Guid? DoctorId { get; set; }   // Chỉ có giá trị nếu Role là Doctor

    }
}
