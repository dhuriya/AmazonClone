using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Wishlist.DTOs;
using AmazonClone.Application.Features.Wishlist.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("Wishlist Management")]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost]
        [SwaggerOperation(
            Summary = "Add product to wishlist",
            Description = "Adds a product to the authenticated user's wishlist."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Product added to wishlist successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        public async Task<IActionResult> AddtoWishlist(AddToWishlistDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _wishlistService.AddToWishlistAsync(userId, dto);
            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Unable to add item to wishlist."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Item added to Wishlist.",
            });
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get my wishlist",
            Description = "Returns the authenticated user's wishlist."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Wishlist retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _wishlistService.GetMyWishlistAsync(userId);
            return Ok(new ApiResponse<WishlistDto>
            {
                Success =true,
                Message = "Wishlist fetched successfully.",
                Data = result
            });
        }
        [HttpDelete("{productId}")]
        [SwaggerOperation(
            Summary = "Remove product from wishlist",
            Description = "Removes a product from the authenticated user's wishlist."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Product removed from wishlist successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Wishlist item not found.")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Wishlist not found."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Item removed successfull."
            });
        }
    }
}
