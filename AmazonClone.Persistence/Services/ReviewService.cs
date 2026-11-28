using AmazonClone.Application.Common.Exceptions;
using AmazonClone.Application.Features.Reviews.DTOs;
using AmazonClone.Application.Features.Reviews.Interfaces;
using AmazonClone.Domain.Entities;
using AmazonClone.Domain.Enums;
using AmazonClone.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Persistence.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ReviewDto> AddReviewAsync(string userId, CreateReviewDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new NotFoundException("User not found.");
            var productExists = await _context.Products.AnyAsync(product => product.Id == dto.ProductId && !product.IsDeleted);
            if (!productExists)
                throw new NotFoundException("Product not found.");
            var hasDeliveredOrder = await _context.Orders
                .AnyAsync(order => order.UserId == userId && order.Status == OrderStatus.Delivered &&
                    !order.IsDeleted && order.OrderItems.Any(item => item.ProductId == dto.ProductId && !item.IsDeleted));
            if (!hasDeliveredOrder)
            {
                throw new BadRequestException("You can review this product after its order has been delivered.");
            }
            var alreadyReviewed = await _context.Reviews.AnyAsync(review => review.UserId == userId &&
                review.ProductId == dto.ProductId && !review.IsDeleted);
            if (alreadyReviewed)
            {
                throw new BadRequestException("You have already reviewed this product.");
            }
            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment.Trim()
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return new ReviewDto
            {
                Id = review.Id,
                UserName = user.UserName ?? string.Empty,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }
        public async Task<List<ReviewDto>> GetReviewsByProductAsync(int productId)
        {
            return await _context.Reviews
                .Include(review => review.User)
                .Where(review =>
                    review.ProductId == productId &&
                    !review.IsDeleted)
                .OrderByDescending(review => review.CreatedOn)
                .Select(review => new ReviewDto
                {
                    Id = review.Id,
                    UserName = review.User.UserName ?? string.Empty,
                    Rating = review.Rating,
                    Comment = review.Comment
                })
                .ToListAsync();
        }
    }
}
