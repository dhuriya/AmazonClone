using AmazonClone.Application.Features.Cart.DTOs;
using AmazonClone.Application.Features.Cart.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    }
}
