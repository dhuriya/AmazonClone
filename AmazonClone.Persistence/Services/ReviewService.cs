using AmazonClone.Application.Features.Reviews.DTOs;
using AmazonClone.Application.Features.Reviews.Interfaces;
using AmazonClone.Domain.Entities;
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
            if (user == null)
                throw new Exception("User not found");
            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
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
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && !r.IsDeleted)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                UserName = r.User.UserName ?? string.Empty,
                Rating = r.Rating,
                Comment = r.Comment
            })
            .ToListAsync();
        }
    }
}
