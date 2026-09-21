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
        [HttpPut("{id}/cancel")]
        [SwaggerOperation(
            Summary = "Cancel an order",
            Description = "Cancels a pending order of the authenticated user and restores product stock."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Order cancelled successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Order cannot be cancelled.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found.")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.CancelOrderAsync(userId, id);
            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Order not found or cannot be cancelled."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order cancelled successfully.",
                Data = null
            });
        }
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get order details",
            Description = "Returns details of a specific order belonging to the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Order retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found.")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetByIdAsync(userId, id);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Order not found."
                });
            }
            return Ok(new ApiResponse<OrderDto>
            {
                Success = true,
                Message = "Order fetched successfully.",
                Data = result
            });
        }
        [HttpGet("{orderId}/tracking")]
        [SwaggerOperation(
            Summary = "Get order tracking information",
            Description = "Returns tracking information for a specific order belonging to the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Tracking information retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found.")]
        public async Task<IActionResult> GetTracking(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetTrackingAsync(userId, orderId);
            if (result == null || !result.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Order not found or no tracking information available."
                });
            }
            return Ok(new ApiResponse<List<OrderTrackingDto>>
            {
                Success = true,
                Message = "Tracking information fetched successfully.",
                Data = result
            });
        }
        [HttpPut("{orderId}/status")]
        [SwaggerOperation(
            Summary = "Update order status",
            Description = "Updates the status of a specific order belonging to the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Order status updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Order status cannot be updated.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found.")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, UpdateOrderStatusDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.UpdateOrderStatusAsync(userId, orderId, dto.Status, dto.Remarks, dto.Location);
            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Order not found or status cannot be updated."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order status updated successfully.",
                Data = null
            });
        }
        [HttpPost("{orderId}/return")]
        [SwaggerOperation(
            Summary = "Request order return",
            Description = "Requests a return for a specific order belonging to the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return request submitted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Return request cannot be submitted.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found.")]
        public async Task<IActionResult> RequestReturn(int orderId, [FromBody] CreateReturnRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "User is not authenticated."
                });
            }
            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Return reason cannot be empty."
                });
            }
            var result = await _orderService.RequestReturnAsync(userId, orderId, dto.Reason);
            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Return request cannot be submitted. Order not found or return not allowed."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Return request submitted successfully."
            });
        }
    }
}
