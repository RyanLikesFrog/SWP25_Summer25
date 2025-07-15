using DataLayer.DbContext;
using DataLayer.Entities;
using DataLayer.Enum;
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
                                 .Include(x => x.Appointment) // Bao gồm Appointment nếu cần
                                 .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);
        }
        public async Task<List<PaymentTransaction>> GetSuccessfulTransactionsByDateRangeAsync(DateTime fromDate, DateTime? toDate)
        {
            var query = _context.PaymentTransactions
                .Include(t => t.Appointment)
                    .ThenInclude(a => a.Patient)
                .Where(t => t.Status == PaymentTransactionStatus.Success);

            if (toDate == null)
            {
                var date = fromDate.Date;
                query = query.Where(t => t.CreatedDate.Date == date);
            }
            else
            {
                var from = fromDate.Date;
                var to = toDate.Value.Date.AddDays(1);
                query = query.Where(t => t.CreatedDate >= from && t.CreatedDate < to);
            }

            return await query.ToListAsync();
        }
    }
}
