using ServiceLayer.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PaymentGateways
{
    public interface IMomoClient
    {
        public Task<MomoCreatePaymentResponse> CreatePaymentAsync(MomoCreatePaymentRequest request);
        public string GenerateSignature(string rawData, string secretKey);
    }
}
