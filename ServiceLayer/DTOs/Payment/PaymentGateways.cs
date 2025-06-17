using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }

// Models/Momo/MomoCreatePaymentRequest.cs
    public class MomoCreatePaymentRequest
    {
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string RequestId { get; set; }
        public string Amount { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string IpnUrl { get; set; }
        public string RequestType { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }

        public string GetSignatureRawData()
        {
            return $"partnerCode={PartnerCode}&accessKey={AccessKey}&requestId={RequestId}&amount={Amount}&orderId={OrderId}&orderInfo={OrderInfo}&returnUrl={ReturnUrl}&ipnUrl={IpnUrl}&requestType={RequestType}&extraData={ExtraData}";
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
