using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Response
{
    public class ARVProtocolDetailResponse
    {
        public Guid ProtocolId { get; set; }
        public string ProtocolName { get; set; }
        public string Description { get; set; }
        public string Indications { get; set; }
        public string Dosage { get; set; }
        public string SideEffects { get; set; }
        public bool IsDefault { get; set; }
        public ARVProtocolType ProtocolType { get; set; }   
    }
}
