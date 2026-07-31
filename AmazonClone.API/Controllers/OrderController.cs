using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Orders.DTOs;
using AmazonClone.Application.Features.Orders.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("Order Management")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("checkout")]
        [SwaggerOperation(
            Summary = "Place an order",
            Description = "Creates a new order from the authenticated user's cart."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Order placed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Cart is empty or request is invalid.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        public async Task<IActionResult> Checkout(CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.CheckoutAsync(userId, dto);
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Order placed successfully.",
                Data = result
            });
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get my orders",
            Description = "Returns all orders placed by the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Orders retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetMyOrdersAsync(userId);
            return Ok(new ApiResponse<List<OrderDto>>
            {
                Success = true,
                Message = "Orders fetched successfully.",
                Data = result
            });
        }
    }
}
