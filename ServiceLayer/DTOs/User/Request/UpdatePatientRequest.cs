using DataLayer.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class UpdatePatientRequest
    {
            public Guid PatientId { get; set; } // ID của người dùng cần cập nhật

            [EmailAddress]
            public string? Email { get; set; }

            [Phone]
            [MaxLength(20)]
            public string? PhoneNumber { get; set; }
            public string? FullName { get; set; }

            [Required]
            public DateTime DateOfBirth { get; set; }

            [Required]
            public Gender Gender { get; set; } // Sử dụng Enum cho giới tính

            [MaxLength(255)]
            public string? Address { get; set; }

            [MaxLength(100)]
            public string? ContactPersonName { get; set; }

            [MaxLength(20)]
            public string? ContactPersonPhone { get; set; }
            public IFormFile? AvatarPicture { get; set; }

    }
}
