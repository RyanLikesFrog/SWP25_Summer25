using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.User.Request
{
    public class PaymentTransactionStatementRequest
    {
        public DateTime FromDate { get; set; } // bắt buộc

        public DateTime? ToDate { get; set; }  
    }
}
