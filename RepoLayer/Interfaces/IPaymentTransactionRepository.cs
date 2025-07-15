using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        public Task AddPaymentTransactionAsync(PaymentTransaction payment);
        public Task<PaymentTransaction?> GetPaymentTransactionByTransactionCodeAsync(string transactionCode);
        public Task<List<PaymentTransaction>> GetSuccessfulTransactionsByDateRangeAsync(DateTime fromDate, DateTime? toDate);
    }
}
