using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Enum;

namespace DataLayer.Entities
{
    public class Blog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Title { get; set; }

        public string? Content { get; set; } // TEXT

        public bool IsApproved { get; set; }

        public DateTime? PublicationDate { get; set; }

        [MaxLength(255)]
        public BlogTag Tags { get; set; }

        [ForeignKey("User")]
        public Guid? AuthorID { get; set; } // Nullable nếu bài viết không liên kết trực tiếp với User
        public virtual User? Author { get; set; }
    }
}
