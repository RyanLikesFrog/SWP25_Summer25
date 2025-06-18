using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Payment
{
    public class MomoSettings
    {
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string ApiEndpoint { get; set; }
        public string ReturnUrl { get; set; }
        public string IpnUrl { get; set; }
        public string RequestType { get; set; }
    }

// Models/Momo/MomoCreatePaymentRequest.cs
    public class MomoCreatePaymentRequest
    {
        [JsonPropertyName("partnerCode")] // Momo thường dùng camelCase
        public string PartnerCode { get; set; }

        [JsonPropertyName("accessKey")]
        public string AccessKey { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("orderInfo")]
        public string OrderInfo { get; set; }

        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("ipnUrl")]
        public string IpnUrl { get; set; }

        [JsonPropertyName("requestType")]
        public string RequestType { get; set; }

        [JsonPropertyName("extraData")]
        public string ExtraData { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; } = "vi"; // Mặc định là tiếng Việt

        public string GetSignatureRawData()
        {
            // CỰC KỲ QUAN TRỌNG: Thứ tự các trường phải KHỚP CHÍNH XÁC với template của Momo (sắp xếp theo a-z)
            // và tên các trường trong chuỗi phải khớp với tên trong tài liệu Momo (camelCase).

            var dataBuilder = new StringBuilder();

            // Sắp xếp theo thứ tự bảng chữ cái (a-z)
            dataBuilder.Append($"accessKey={AccessKey}");
            dataBuilder.Append($"&amount={Amount}");
            dataBuilder.Append($"&extraData={ExtraData}");
            dataBuilder.Append($"&ipnUrl={IpnUrl}");
            dataBuilder.Append($"&orderId={OrderId}");
            dataBuilder.Append($"&orderInfo={OrderInfo}");
            dataBuilder.Append($"&partnerCode={PartnerCode}");
            dataBuilder.Append($"&redirectUrl={RedirectUrl}"); // Phải là "redirectUrl"
            dataBuilder.Append($"&requestId={RequestId}");
            dataBuilder.Append($"&requestType={RequestType}"); // Phải có "requestType"

            return dataBuilder.ToString();
        }
    }

    // Models/Momo/MomoCreatePaymentResponse.cs
    public class MomoCreatePaymentResponse
    {
        [JsonPropertyName("partnerCode")] // Tên trường trong JSON: partnerCode
        public string PartnerCode { get; set; }

        [JsonPropertyName("orderId")] // Tên trường trong JSON: orderId
        public string OrderId { get; set; }

        [JsonPropertyName("requestId")] // Tên trường trong JSON: requestId
        public string RequestId { get; set; }

        [JsonPropertyName("amount")] // Tên trường trong JSON: amount, kiểu int/long
        public long Amount { get; set; }

        [JsonPropertyName("responseTime")] // Tên trường trong JSON: responseTime, kiểu long
        public long ResponseTime { get; set; }

        [JsonPropertyName("message")] // Tên trường trong JSON: message
        public string Message { get; set; }

        [JsonPropertyName("resultCode")] // Tên trường trong JSON: resultCode, kiểu int
        public int ResultCode { get; set; }

        [JsonPropertyName("payUrl")] // Tên trường trong JSON: payUrl
        public string PayUrl { get; set; }

        [JsonPropertyName("deeplink")] // Tên trường trong JSON: deeplink
        public string Deeplink { get; set; }

        [JsonPropertyName("qrCodeUrl")] // Tên trường trong JSON: qrCodeUrl
        public string QrCodeUrl { get; set; }

        // Các thuộc tính sau có thể xuất hiện trong response lỗi hoặc các loại response khác
        // nhưng không có trong mẫu response thành công bạn cung cấp.
        // Giữ lại để đảm bảo tính bao quát nếu chúng có thể xuất hiện trong các tình huống khác.
        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("localMessage")]
        public string LocalMessage { get; set; }

        [JsonPropertyName("transId")]
        public string TransId { get; set; }

        [JsonPropertyName("extraData")]
        public string ExtraData { get; set; }

        [JsonPropertyName("requestType")]
        public string RequestType { get; set; }
    }
    // Models/Momo/MomoCallbackRequest.cs (Giữ nguyên)
    public class MomoCallbackRequest
    {
        [JsonPropertyName("partnerCode")]
        public string PartnerCode { get; set; } = string.Empty; // Gán mặc định để tránh null

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public long Amount { get; set; } // Kiểu Long theo tài liệu

        [JsonPropertyName("storeId")]
        public string? StoreId { get; set; } // Optional, có thể null

        [JsonPropertyName("orderInfo")]
        public string OrderInfo { get; set; } = string.Empty;

        [JsonPropertyName("partnerUserId")]
        public string? PartnerUserId { get; set; } // Có thể null

        [JsonPropertyName("orderType")]
        public string OrderType { get; set; } = string.Empty;

        [JsonPropertyName("transId")]
        public long TransId { get; set; } // Kiểu Long theo tài liệu

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("payType")]
        public string? PayType { get; set; } // Có thể null

        [JsonPropertyName("responseTime")]
        public long ResponseTime { get; set; }

        [JsonPropertyName("extraData")]
        public string? ExtraData { get; set; } // Có thể null

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("paymentOption")]
        public string? PaymentOption { get; set; } // Có thể null

        [JsonPropertyName("userFee")]
        public long? UserFee { get; set; } // Có thể null, kiểu Long

        // promotionInfo là List và default là null, cần có class riêng nếu muốn parse chi tiết
        // Nếu không, có thể bỏ qua hoặc để là object/JsonElement
        // [JsonPropertyName("promotionInfo")]
        // public List<PromotionInfo>? PromotionInfo { get; set; }
    }

    // Nếu bạn cần parse promotionInfo, định nghĩa class này
    // public class PromotionInfo
    // {
    //     // Định nghĩa các thuộc tính của promotionInfo nếu tài liệu Momo cung cấp
    // }
}
