using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Enum;

namespace DataLayer.Entities
{
    public class PaymentTransaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "VND";

        [Required]
        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;

        // --- Các trường liên quan đến MoMo ---
        // --- Các trường liên quan đến MoMo ---

        // Đây là "requestId" từ MoMo. Mỗi yêu cầu tạo giao dịch MoMo có một requestId duy nhất.
        public string? MomoRequestId { get; set; }

        // Đây là "transId" từ MoMo, định danh giao dịch trên hệ thống MoMo sau khi hoàn tất.
        public string? MomoTransactionId { get; set; }

        // "orderId" từ MoMo, thường là orderId của hệ thống bạn truyền sang MoMo
        public string? MomoOrderId { get; set; }

        // Mã kết quả từ MoMo (ResultCode)
        public string? MomoResultCode { get; set; }

        // Thông báo từ MoMo (Message)
        public string? MomoMessage { get; set; }

        // Chữ ký (Signature) của giao dịch từ MoMo (khi nhận IPN)
        public string? MomoSignature { get; set; }

        // URL thanh toán mà MoMo trả về (payUrl)
        public string? PaymentUrl { get; set; }

        // Thời gian MoMo trả về kết quả (responseTime)
        public DateTime? MomoResponseTime { get; set; }

        // Thông tin bổ sung từ MoMo (ExtraData)
        public string? MomoExtraData { get; set; }

        // --- Các trường chung ---
        public string? TransactionCode { get; set; } // Mã giao dịch nội bộ của bạn, thường dùng làm MomoOrderId ban đầu

        public string? Message { get; set; } // Thông báo chung cho giao dịch (có thể trùng với MomoMessage)

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; } // Thời gian giao dịch được xử lý xong (khi nhận callback)

        // --- Mối quan hệ 1:1 với Appointment ---
        // Mỗi PaymentTransaction thuộc về một Appointment duy nhất
        [Required]
        [ForeignKey("AppointmentId")]
        public Guid AppointmentId { get; set; } // Khóa ngoại tới Appointment
        public virtual Appointment? Appointment { get; set; }
    }
}
