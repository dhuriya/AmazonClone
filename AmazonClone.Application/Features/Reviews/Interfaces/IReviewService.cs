using AmazonClone.Application.Features.Reviews.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Reviews.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(string userId, CreateReviewDto dto);
        Task<List<ReviewDto>> GetReviewsByProductAsync(int productId);
    }
}
