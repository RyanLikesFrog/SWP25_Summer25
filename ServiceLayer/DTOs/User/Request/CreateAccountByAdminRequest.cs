using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class CreateAccountByAdminRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid UserRole.")]
        public UserRole Role { get; set; }

        // Doctor Specific Fields (nullable for Staff/Manager)
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string? Specialization { get; set; }
        public string? Qualifications { get; set; }
        public string? Experience { get; set; }
        public string? Bio { get; set; }

        [StringLength(255, ErrorMessage = "Profile Picture URL cannot exceed 255 characters.")]
        public string? ProfilePictureURL { get; set; }
    }
}
