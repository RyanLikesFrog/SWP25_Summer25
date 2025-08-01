using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Patient.Response
{
    public class ReminderDetailDto
    {
        public Guid StageId { get; set; }
        public string? StageName { get; set; }
        public int StageNumber { get; set; }
        public string? Description { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public Guid? PatientTreatmentProtocolId { get; set; }

        // Thông tin về thuốc cụ thể
        public Guid PrescriptionItemId { get; set; }
        public string? DrugName { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
    }
}
