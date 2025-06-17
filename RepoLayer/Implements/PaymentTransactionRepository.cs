using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly SWPSU25Context _context;
        public PaymentTransactionRepository(SWPSU25Context context) 
        {
            _context = context;
        }
        public async Task AddPaymentTransactionAsync(PaymentTransaction payment)
        {
            await _context.PaymentTransactions.AddAsync(payment);
        }

        public async Task<PaymentTransaction?> GetPaymentTransactionByTransactionCodeAsync(string transactionCode)
        {
            // Sử dụng FirstOrDefaultAsync để lấy một bản ghi duy nhất hoặc null nếu không tìm thấy
            // Bao gồm (Include) Appointment nếu bạn muốn truy cập nó ngay sau khi lấy transaction
            return await _context.PaymentTransactions
                                 .Include(pt => pt.Appointment) // Tải Appointment cùng với PaymentTransaction
                                 .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);
        }
    }
}
