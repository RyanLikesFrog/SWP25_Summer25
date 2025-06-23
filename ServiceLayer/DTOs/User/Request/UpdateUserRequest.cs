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
    public class UpdateUserRequest
    {// Các trường thông tin chung của User
        public Guid UserId { get; set; } // ID của người dùng cần cập nhật
        public string? Username { get; set; } // Optional: If you allow username changes

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public UserRole? Role { get; set; } // Admin có thể thay đổi Role

        // Các trường thông tin riêng của Doctor (sẽ là null nếu người dùng không phải Doctor)
        [MaxLength(100)]
        public string? FullName { get; set; } // Đối với Doctor, đây là tên đầy đủ của họ

        [MaxLength(100)]
        public string? Specialization { get; set; }

        public string? Qualifications { get; set; } // TEXT
        public string? Experience { get; set; } // TEXT
        public string? Bio { get; set; } // TEXT
        public IFormFile? AvatarPicture { get; set; }
    }
}
