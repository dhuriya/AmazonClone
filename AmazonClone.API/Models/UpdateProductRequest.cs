namespace AmazonClone.API.Models
{
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
    }
}
