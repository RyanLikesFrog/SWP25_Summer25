using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class CreatePrescriptionRequest
    {
        [Required]
        public string DrugName { get; set; }

        [Required]
        public string Dosage { get; set; }

        public int Quantity { get; set; }

        public string? Note { get; set; }
    }
}
