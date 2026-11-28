using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Home
{
    public class SubmitProductReviewViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid product.")]
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Choose a rating from 1 to 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Enter your review.")]
        [StringLength(1000, MinimumLength = 5,ErrorMessage = "Your review must be between 5 and 1000 characters.")]
        public string Comment { get; set; } = string.Empty;
    }
}
