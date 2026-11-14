using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Admin
{
    public class ProductCreateViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ExistingImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public List<CategoryAdminViewModel> Categories { get; set; } = new();
    }
}
