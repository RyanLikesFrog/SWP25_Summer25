using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class ARVProtocol
    {
        [Key]
        public Guid ProtocolId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ProtocolName { get; set; }

        public string? Description { get; set; } // TEXT
        public string? Indications { get; set; } // TEXT
        public string? Dosage { get; set; } // TEXT
        public string? SideEffects { get; set; } // TEXT

        public bool IsDefault { get; set; }
        
        [Required]
        public ARVProtocolType ProtocolType { get; set; } // Enum to define the type of ARV protocol

        // Navigation property cho mối quan hệ 1-N
        public virtual ICollection<PatientTreatmentProtocol>? PatientTreatmentProtocols{ get; set; }
    }
}
