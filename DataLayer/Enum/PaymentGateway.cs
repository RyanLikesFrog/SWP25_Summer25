using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Enum
{
    public enum PaymentGateway
    {
        None = 0,
        MoMo = 1,
        VnPay = 2,
        PayPal = 3,
        Stripe = 4
    }
}
