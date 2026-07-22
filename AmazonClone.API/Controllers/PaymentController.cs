using AmazonClone.Application.Features.Payments.DTOs;
using AmazonClone.Application.Features.Payments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;            
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _paymentService.CreatePaymentAsync(userId, dto);
            return Ok(result);
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> Get(int orderId)
        {
            var result = _paymentService.GetPaymentAsync(orderId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
