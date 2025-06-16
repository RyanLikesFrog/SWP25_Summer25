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

        // **ĐIỂM SỬA ĐỔI: Loại bỏ AppointmentId và virtual Appointment ở đây
        // Nếu bạn muốn mối quan hệ một-một chặt chẽ hơn, PaymentTransactionId của Appointment
        // sẽ là khóa chính của PaymentTransaction. Hoặc để như hiện tại
        // và đảm bảo logic ứng dụng chỉ tạo 1 giao dịch cho 1 cuộc hẹn.**

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "VND";

        // Vì chỉ dùng VNPAY, có thể bỏ Enum Gateway hoặc đặt mặc định
        [Required]
        public PaymentGateway Gateway { get; set; } = PaymentGateway.VnPay; // Mặc định là VNPAY

        [Required]
        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;

        public string? TransactionCode { get; set; } // Mã giao dịch nội bộ (vnp_TxnRef)
        public string? PaymentGatewayTransactionId { get; set; } // Mã giao dịch từ VNPAY (vnp_TransactionNo)

        public string? Message { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; }

        public string? PaymentUrl { get; set; }

        public string? CallbackData { get; set; }

        public string? BankCode { get; set; }
        public string? CardType { get; set; }
        public string? PayDate { get; set; }
        public string? ResponseCode { get; set; }
        public string? TmnCode { get; set; }
    }
}
