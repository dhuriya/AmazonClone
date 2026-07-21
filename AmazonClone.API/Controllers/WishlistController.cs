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
        public async Task<IActionResult> Add(AddToWishlistDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _wishlistService.AddToWishlistAsync(userId, dto);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _wishlistService.GetMyWishlistAsync(userId);
            return Ok(result);
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            if (!result)
                return BadRequest();
            return Ok("Item removed successfull.");
        }
    }
}
