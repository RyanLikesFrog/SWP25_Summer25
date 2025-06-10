using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class CreateARVProtocolRequest
    {
        [Required(ErrorMessage = "Tên giao thức là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên giao thức không được vượt quá 100 ký tự.")]
        public string ProtocolName { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public string? Indications { get; set; } 
        public string? Dosage { get; set; }
        public string? SideEffects { get; set; } 

        [Required(ErrorMessage = "Thuộc tính 'IsDefault' là bắt buộc.")]
        public bool IsDefault { get; set; }

        [Required(ErrorMessage = "Loại giao thức là bắt buộc.")]
        public ARVProtocolType ProtocolType { get; set; }
    }
}
