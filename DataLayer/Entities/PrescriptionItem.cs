using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    public class PrescriptionItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Prescription")]
        public Guid PrescriptionId { get; set; }
        public virtual Prescription? Prescription { get; set; }

        [Required]
        public string? DrugName { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
    }

}