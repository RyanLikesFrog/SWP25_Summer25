using RepoLayer.Interfaces;
using ServiceLayer.DTOs.Payment;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;

        public PaymentTransactionService(IPaymentTransactionRepository paymentTransactionRepository)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
        }

        public async Task<PaymentTransactionStatementResponse> GetPaymentTransactionStatementAsync(PaymentTransactionStatementRequest request)
        {
            var transactions = await _paymentTransactionRepository.GetSuccessfulTransactionsByDateRangeAsync(request.FromDate, request.ToDate);

            var response = new PaymentTransactionStatementResponse
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TotalAmount = transactions.Sum(t => t.Amount),
                Transactions = transactions.Select(t => new PaymentTransactionDTO
                {
                    Amount = t.Amount,
                    Status = t.Status.ToString(),
                    Message = t.Message,
                    CreatedDate = t.CreatedDate,
                    PatientFullName = t.Appointment?.Patient?.FullName ?? "N/A"
                }).ToList()
            };

            return response;
        }
    }
}
