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
        public string PartnerCode { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string PayUrl { get; set; }
        public string QrCodeUrl { get; set; }
        public string Signature { get; set; }
        public string ErrorCode { get; set; }
        public string LocalMessage { get; set; }
        public string TransId { get; set; }
        public long ResponseTime { get; set; }
        public string? ExtraData { get; set; }
        public string RequestType { get; set; }
    }
    // Models/Momo/MomoCallbackRequest.cs (Giữ nguyên)
    public class MomoCallbackRequest
    {
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public string Amount { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public long ResponseTime { get; set; }
        public string? ExtraData { get; set; }
        public string Signature { get; set; }
        public string TransId { get; set; }
    }
}
