using AmazonClone.Application.Features.Cart.DTOs;
using AmazonClone.Application.Features.Cart.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCartAsync(userId, dto);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.GetMyCartAsync(userId);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity(UpdateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.UpdateQuantityAsync(userId, dto);
            if (!result)
                return BadRequest();
            return Ok("Cart updated successfully.");
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.RemoveItemAsync(userid, productId);
            if (!result)
                return BadRequest();
            return Ok("Item removed successfully.");
        }
    }
}
