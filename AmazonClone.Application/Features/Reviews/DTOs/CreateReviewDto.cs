using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Reviews.DTOs
{
    public class CreateReviewDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select a valid product.")]
        public int ProductId { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Enter a review comment.")]
        [StringLength(1000, MinimumLength = 5,ErrorMessage = "Your review must be between 5 and 1000 characters.")]
        public string Comment { get; set; } = string.Empty;
    }
}
