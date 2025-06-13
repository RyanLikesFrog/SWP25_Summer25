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
        public int StageNumber { get; set; } // Thứ tự của giai đoạn trong phác đồ
        public string? Description { get; set; } // Mô tả của giai đoạn điều trị
        public DateTime ReminderDateTime { get; set; } // Thời điểm nhắc nhở cụ thể
        public Guid? PatientTreatmentProtocolId { get; set; } // Để Front-end có thể liên kết với bệnh nhân
    }
}
