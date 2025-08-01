using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Patient.Response
{
    public class PrescriptionItemDto
    {
        [Required]
        public string DrugName { get; set; }

        [Required]
        public string Dosage { get; set; }
    }
}
