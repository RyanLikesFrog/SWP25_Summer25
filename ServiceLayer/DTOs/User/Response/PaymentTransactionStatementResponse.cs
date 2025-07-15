using ServiceLayer.DTOs.Payment;

namespace ServiceLayer.DTOs.User.Response
{
    public class PaymentTransactionStatementResponse
    {
        public decimal TotalAmount { get; set; } // tổng tiền các transaction

        public List<PaymentTransactionDTO> Transactions { get; set; } = new();

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
