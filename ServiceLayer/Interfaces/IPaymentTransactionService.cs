using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IPaymentTransactionService
    {
        public Task<PaymentTransactionStatementResponse> GetPaymentTransactionStatementAsync(PaymentTransactionStatementRequest request);
    }
}
