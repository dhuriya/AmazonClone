using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Payments.DTOs;
using AmazonClone.Application.Features.Payments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("Payment Management")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;            
        }
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create payment",
            Description = "Creates a payment for the specified order."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found.")]
        public async Task<IActionResult> Create(CreatePaymentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _paymentService.CreatePaymentAsync(userId, dto);
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "payment completed successfully.",
                Data = result
            });
        }
        [HttpGet("{orderId}")]
        [SwaggerOperation(
            Summary = "Get payment details",
            Description = "Returns payment details for the specified order."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Payment not found.")]
        public async Task<IActionResult> Get(int orderId)
        {
            var result = await _paymentService.GetPaymentAsync(orderId);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "payment not found."
                });
            }
            return Ok(new ApiResponse<PaymentDto>
            {
                Success = true,
                Message = "Payment fetched successfully.",
                Data = result
            });
        }
    }
}
