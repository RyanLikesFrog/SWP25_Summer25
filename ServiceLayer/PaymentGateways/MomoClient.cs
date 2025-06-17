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
            request.ReturnUrl = _settings.ReturnUrl;
            request.IpnUrl = _settings.IpnUrl;
            request.RequestType = "captureMoMoWallet";

            request.RequestId = Guid.NewGuid().ToString("N");

            string rawData = request.GetSignatureRawData();
            request.Signature = GenerateSignature(rawData, _settings.SecretKey);

            _logger.LogInformation("Momo Request Payload (to be sent): {Payload}", JsonSerializer.Serialize(request));

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("", content);

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
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
