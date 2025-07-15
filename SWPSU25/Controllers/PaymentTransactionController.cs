using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
namespace SWPSU25.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentTransactionController(IPaymentTransactionService paymentTransactionService)
        {
            _paymentTransactionService = paymentTransactionService;
        }

        [HttpPost("get-payment-transaction")]
        public async Task<IActionResult> GetTransactionStatement([FromBody] PaymentTransactionStatementRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (request.ToDate != null && request.ToDate < request.FromDate)
                return BadRequest("ToDate must be greater than or equal to FromDate");

            var result = await _paymentTransactionService.GetPaymentTransactionStatementAsync(request);

            return Ok(result);
        }
    }
}
