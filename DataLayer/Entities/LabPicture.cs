using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class LabPicture
    {
        [Key]
        public Guid Id { get; set; }
        public string? LabPictureUrl { get; set; } // Tên tệp hình ảnh
        public string? LabPictureName { get; set; } // Tên hiển thị của hình ảnh
        public bool isActive { get; set; } = true; // Mô tả hình ảnh

        [ForeignKey("LabResult")]
        public Guid LabResultId { get; set; } // Khóa ngoại tới LabResult
        public virtual LabResult? LabResult { get; set; } // Navigation property
    }
}
