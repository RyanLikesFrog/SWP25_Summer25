using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceLayer.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServiceLayer.PaymentGateways
{
    public class VnPayPaymentClient
    {
        private readonly VnPaySettings _settings;
        private readonly ILogger<VnPayPaymentClient> _logger;

        public VnPayPaymentClient(IConfiguration configuration, ILogger<VnPayPaymentClient> logger)
        {
            _settings = configuration.GetSection("VnPaySettings").Get<VnPaySettings>();
            _logger = logger;
            // Đảm bảo settings không null
            if (_settings == null || string.IsNullOrEmpty(_settings.TmnCode) || string.IsNullOrEmpty(_settings.HashSecret))
            {
                throw new InvalidOperationException("VNPAY settings are not configured properly.");
            }
        }

        public string GetReturnUrl() => _settings.ReturnUrl;
        public string GetNotifyUrl() => _settings.NotifyUrl;

        /// <summary>
        /// Tạo URL thanh toán VNPAY
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thanh toán</param>
        /// <param name="clientIpAddress">Địa chỉ IP của client gửi request (lấy từ HttpContext)</param>
        /// <returns>URL để chuyển hướng người dùng đến cổng VNPAY</returns>
        public string CreatePaymentUrl(VnPayCreatePaymentRequest request, string clientIpAddress)
        {
            var vnp_Params = new SortedList<string, string>();

            vnp_Params.Add("vnp_Version", "2.1.0"); // Phiên bản API
            vnp_Params.Add("vnp_Command", "pay"); // Lệnh thanh toán
            vnp_Params.Add("vnp_TmnCode", _settings.TmnCode); // Mã merchant

            vnp_Params.Add("vnp_Amount", (request.Amount).ToString()); // Số tiền (VNĐ * 100)
            vnp_Params.Add("vnp_CurrCode", "VND"); // Đơn vị tiền tệ
            vnp_Params.Add("vnp_TxnRef", request.OrderId); // Mã tham chiếu giao dịch của bạn
            vnp_Params.Add("vnp_OrderInfo", request.OrderInfo); // Mô tả đơn hàng
            vnp_Params.Add("vnp_OrderType", "other"); // Loại đơn hàng (ví dụ: other, billpay, sale)

            vnp_Params.Add("vnp_Locale", request.Locale); // Ngôn ngữ (vn/en)
            vnp_Params.Add("vnp_ReturnUrl", _settings.ReturnUrl); // URL nhận kết quả trên FE
            vnp_Params.Add("vnp_IpAddr", clientIpAddress); // IP của khách hàng
            vnp_Params.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); // Thời gian tạo đơn hàng
            vnp_Params.Add("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss")); // Thời gian hết hạn (ví dụ: 15 phút)

            // VNPAY NotifyUrl (Webhook) - Quan trọng để nhận kết quả chính xác
            vnp_Params.Add("vnp_Url", _settings.NotifyUrl);

            // Building the query string for hash calculation and URL
            StringBuilder query = new StringBuilder();
            foreach (var kvp in vnp_Params)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    // VNPAY yêu cầu URL encode cả key và value
                    query.AppendFormat("{0}={1}&", kvp.Key, HttpUtility.UrlEncode(kvp.Value, Encoding.UTF8));
                }
            }

            string payUrl = _settings.PaymentGatewayUrl + "?" + query.ToString();
            string hashData = query.ToString();

            // Xóa ký tự '&' cuối cùng nếu có
            if (hashData.Length > 0 && hashData[hashData.Length - 1] == '&')
            {
                hashData = hashData.Substring(0, hashData.Length - 1);
            }

            // **Tính toán VNPAY Secure Hash (HMACSHA512)**
            string vnp_SecureHash = HmacSHA512(_settings.HashSecret, hashData);
            payUrl += "vnp_SecureHash=" + vnp_SecureHash;

            _logger.LogInformation("VNPAY Payment URL generated: {PayUrl}", payUrl);
            return payUrl;
        }

        /// <summary>
        /// Xử lý và xác minh kết quả thanh toán từ callback của VNPAY (IPN hoặc ReturnUrl)
        /// </summary>
        /// <param name="queryParams">Dictionary chứa các tham số từ Query String của VNPAY</param>
        /// <returns>Đối tượng VnPayPaymentResult chứa thông tin kết quả</returns>
        public VnPayPaymentResult ProcessPaymentResult(Dictionary<string, string> queryParams)
        {
            var result = new VnPayPaymentResult();
            result.RawData = new Dictionary<string, string>(queryParams); // Lưu toàn bộ data để debug

            try
            {
                // Bước 1: Lấy Secure Hash từ query params và xóa nó khỏi dictionary
                string vnp_SecureHash = queryParams.ContainsKey("vnp_SecureHash") ? queryParams["vnp_SecureHash"] : null;
                if (queryParams.ContainsKey("vnp_SecureHash"))
                {
                    queryParams.Remove("vnp_SecureHash");
                }

                // Sắp xếp các tham số theo thứ tự alphabet (theo quy định của VNPAY)
                var sortedParams = new SortedList<string, string>(queryParams);

                // Bước 2: Xây dựng chuỗi hash data từ các tham số đã sắp xếp
                StringBuilder hashDataBuilder = new StringBuilder();
                foreach (var kvp in sortedParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        hashDataBuilder.AppendFormat("{0}={1}&", kvp.Key, HttpUtility.UrlEncode(kvp.Value, Encoding.UTF8));
                    }
                }
                string hashData = hashDataBuilder.ToString();
                if (hashData.Length > 0 && hashData[hashData.Length - 1] == '&')
                {
                    hashData = hashData.Substring(0, hashData.Length - 1);
                }

                // Bước 3: Tính toán lại Secure Hash và so sánh
                string expectedHash = HmacSHA512(_settings.HashSecret, hashData);

                if (!string.Equals(expectedHash, vnp_SecureHash, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("VNPAY callback signature mismatch! Expected: {Expected}, Actual: {Actual}", expectedHash, vnp_SecureHash);
                    result.ResponseCode = "97"; // Invalid signature
                    result.Message = "Invalid signature: " + expectedHash + " != " + vnp_SecureHash;
                    result.SecureHashValid = false;
                    return result;
                }
                result.SecureHashValid = true; // Hash hợp lệ

                // Bước 4: Đọc kết quả từ các tham số VNPAY
                result.OrderId = queryParams.ContainsKey("vnp_TxnRef") ? queryParams["vnp_TxnRef"] : null;
                result.TransactionNo = queryParams.ContainsKey("vnp_TransactionNo") ? queryParams["vnp_TransactionNo"] : null;
                result.ResponseCode = queryParams.ContainsKey("vnp_ResponseCode") ? queryParams["vnp_ResponseCode"] : null;
                result.Amount = queryParams.ContainsKey("vnp_Amount") ? long.Parse(queryParams["vnp_Amount"]) : 0;
                result.BankCode = queryParams.ContainsKey("vnp_BankCode") ? queryParams["vnp_BankCode"] : null;
                result.PayDate = queryParams.ContainsKey("vnp_PayDate") ? queryParams["vnp_PayDate"] : null;
                // Có thể lấy vnp_Message hoặc tự định nghĩa theo ResponseCode
                result.Message = GetVnPayResponseMessage(result.ResponseCode);

                _logger.LogInformation("VNPAY Payment result processed: OrderId={OrderId}, ResponseCode={ResponseCode}", result.OrderId, result.ResponseCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPAY payment result. Raw data: {RawData}", System.Text.Json.JsonSerializer.Serialize(queryParams));
                result.ResponseCode = "99"; // Lỗi không xác định
                result.Message = "Unknown error during VNPAY callback processing";
                result.SecureHashValid = false;
            }
            return result;
        }

        /// <summary>
        /// Hàm băm HMACSHA512
        /// </summary>
        private string HmacSHA512(string key, string input)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                foreach (byte b in hashBytes)
                {
                    hash.Append(b.ToString("X2")); // "X2" for uppercase hex
                }
            }
            return hash.ToString();
        }

        /// <summary>
        /// Hàm hỗ trợ lấy mô tả từ mã lỗi VNPAY
        /// </summary>
        private string GetVnPayResponseMessage(string? responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "01" => "Giao dịch đã tồn tại",
                "02" => "Merchant không hợp lệ",
                "03" => "Sai mã checksum",
                "04" => "Phiên giao dịch không hợp lệ",
                "05" => "Số tiền không hợp lệ",
                "06" => "IP Address không hợp lệ",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, gian lận)",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Vui lòng thực hiện lại giao dịch.",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
                "13" => "Giao dịch không thành công do: Sai OTP (mật khẩu dùng một lần).",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch.",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách vượt quá hạn mức giao dịch trong ngày.",
                "75" => "Ngân hàng đang bảo trì.",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu quá số lần quy định.",
                "94" => "Giao dịch bị trùng lặp do phía Merchant gửi sang VNPAY.",
                "97" => "Chữ ký không hợp lệ",
                "99" => "Lỗi không xác định.",
                _ => "Giao dịch không thành công."
            };
        }
    }
}
