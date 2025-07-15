using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Payment
{
    public class PaymentTransactionDTO
    {
    public decimal Amount { get; set; }

    public string Status { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedDate { get; set; }

    public string PatientFullName { get; set; } = string.Empty;
    }
}
