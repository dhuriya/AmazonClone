using AmazonClone.Application.Features.Reviews.DTOs;
using AmazonClone.Application.Features.Reviews.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _reviewService.AddReviewAsync(userId, dto);
            return Ok(result); 
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var result = await _reviewService.GetReviewsByProductAsync(productId);
            return Ok(result);
        }
    }
}
