using DataLayer.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Requests
{
    public class UpdateARVProtocolRequest
    {
        [Required]
        public Guid ProtocolId { get; set; }

        [MaxLength(100)]
        public string? ProtocolName { get; set; }

        public string? Description { get; set; }

        public string? Indications { get; set; }

        public string? Dosage { get; set; }

        public string? SideEffects { get; set; }

        public bool? IsDefault { get; set; }

        public ARVProtocolType? ProtocolType { get; set; }
    }
}
