using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class User
    {
        [Key] // Đánh dấu UserID là khóa chính
        public Guid Id { get; set; }

        [Required] // Yêu cầu trường Username không được null
        [MaxLength(50)] // Giới hạn độ dài tối đa 50 ký tự
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; } // Sử dụng Enum cho vai trò người dùng

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Mặc định là thời gian hiện tại UTC
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties cho mối quan hệ 1-1 (nếu có)
        public virtual Patient? Patient { get; set; }    
        public virtual Doctor? Doctor { get; set; }

        public bool isActive { get; set; } = true; 

        // Navigation property cho mối quan hệ 1-N với BlogPosts
        public virtual ICollection<Blog>? Blog { get; set; }
    }
}
