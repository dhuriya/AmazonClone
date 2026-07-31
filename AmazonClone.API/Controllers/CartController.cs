using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Cart.DTOs;
using AmazonClone.Application.Features.Cart.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("Cart Management")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost]
        [SwaggerOperation(
            Summary = "Add product to cart",
            Description = "Adds a product to the authenticated user's shopping cart."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Product added to cart successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCartAsync(userId, dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Item added to cart.",
               
            });
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get my cart",
            Description = "Returns the authenticated user's shopping cart."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cart retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.GetMyCartAsync(userId);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Cart fetched successfully.",
                Data = result
            });
        }
        [HttpPut]
        [SwaggerOperation(
            Summary = "Update cart item quantity",
            Description = "Updates the quantity of a product in the shopping cart."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cart updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Cart item not found.")]
        public async Task<IActionResult> UpdateQuantity(UpdateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.UpdateQuantityAsync(userId, dto);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cart item not found."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cart updated successfully."
            });
        }
        [HttpDelete("{productId}")]
        [SwaggerOperation(
            Summary = "Remove product from cart",
            Description = "Removes a product from the authenticated user's shopping cart."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Product removed from cart successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Cart item not found.")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.RemoveItemAsync(userid, productId);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cart item not found."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Item removed successfully."
            });
        }
    }
}
