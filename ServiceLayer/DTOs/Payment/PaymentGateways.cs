using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Payment
{
    public class VnPaySettings
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string PaymentGatewayUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
    }

    public class VnPayCreatePaymentRequest
    {
        public long Amount { get; set; } // Số tiền (đã nhân 100 theo yêu cầu VNPAY)
        public string OrderId { get; set; } // Mã đơn hàng của bạn (vnp_TxnRef)
        public string OrderInfo { get; set; } // Mô tả đơn hàng (vnp_OrderInfo)
        public string ReturnUrl { get; set; } // vnp_ReturnUrl
        public string Locale { get; set; } = "vn"; // "vn" hoặc "en" (vnp_Locale)
        // Bạn có thể thêm các trường VNPAY khác như vnp_BankCode nếu muốn cho phép chọn ngân hàng từ đầu
    }

    public class VnPayPaymentResult
    {
        public string? OrderId { get; set; } // vnp_TxnRef (Mã đơn hàng của bạn)
        public string? TransactionNo { get; set; } // vnp_TransactionNo (ID giao dịch của VNPAY)
        public string? ResponseCode { get; set; } // vnp_ResponseCode (mã phản hồi của VNPAY, "00" là thành công)
        public string? Message { get; set; } // Tin nhắn tương ứng với ResponseCode
        public long Amount { get; set; } // Số tiền
        public string? BankCode { get; set; } // vnp_BankCode
        public string? PayDate { get; set; } // vnp_PayDate
        public bool SecureHashValid { get; set; } // Kết quả kiểm tra chữ ký Hash
        public Dictionary<string, string> RawData { get; set; } = new Dictionary<string, string>(); // Lưu toàn bộ data để debug
    }
}
