using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceLayer.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ServiceLayer.PaymentGateways
{
    public class MomoClient : IMomoClient
    {
        private readonly MomoSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MomoClient> _logger;

        public MomoClient(IOptions<MomoSettings> settings, HttpClient httpClient, ILogger<MomoClient> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_settings.ApiEndpoint);
        }

        public async Task<MomoCreatePaymentResponse> CreatePaymentAsync(MomoCreatePaymentRequest request)
        {
            request.PartnerCode = _settings.PartnerCode;
            request.AccessKey = _settings.AccessKey;
            request.RedirectUrl = _settings.ReturnUrl;
            request.IpnUrl = _settings.IpnUrl;
            request.RequestType = _settings.RequestType;
            request.RequestId = Guid.NewGuid().ToString("N");
            request.Lang = "vi"; // Mặc định là tiếng Việt

            string rawData = request.GetSignatureRawData();
            request.Signature = GenerateSignature(rawData, _settings.SecretKey);
            // <-- THÊM DÒNG NÀY ĐỂ DEBUG -->
            var jsonPayloadToSend = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true }); // Thêm WriteIndented để dễ đọc
            _logger.LogInformation("Momo Request Payload (DEBUG - to be sent): {Payload}", jsonPayloadToSend);
            // <--------------------------->
            _logger.LogInformation("Momo Request Payload (to be sent): {Payload}", JsonSerializer.Serialize(request));

            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Momo Response Content: {Content}", responseContent);
                var momoResponse = JsonSerializer.Deserialize<MomoCreatePaymentResponse>(responseContent);

                return momoResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Momo API call failed: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Momo API call failed with status code {response.StatusCode} and content: {errorContent}");
            }
        }

        public string GenerateSignature(string rawData, string secretKey)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(rawData);
            Byte[] keyBytes = encoding.GetBytes(secretKey);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
