using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Reviews.DTOs;
using AmazonClone.Application.Features.Reviews.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Review Management")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Add a product review",
            Description = "Adds a review and rating for a product by the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Review added successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found.")]
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _reviewService.AddReviewAsync(userId, dto);
            return Ok(new ApiResponse<ReviewDto>
            {
                Success = true,
                Message = "Review added successfully.",
                Data = result
            }); 
        }
        [HttpGet("{productId}")]
        [SwaggerOperation(
            Summary = "Get product reviews",
            Description = "Returns all reviews for the specified product."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Reviews retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found.")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var result = await _reviewService.GetReviewsByProductAsync(productId);
            return Ok(new ApiResponse<List<ReviewDto>>
            {
                Success = true,
                Message = result.Any()?"Reviews fetched successfully."
                : "No reviews found.",
                Data = result
            });
        }
    }
}
