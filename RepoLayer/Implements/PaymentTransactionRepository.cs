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
    }
}
