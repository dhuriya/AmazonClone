using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Wishlist.DTOs;
using AmazonClone.Application.Features.Wishlist.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost]
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
