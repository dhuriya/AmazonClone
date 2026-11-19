namespace AmzonClone.Web.Models.Home
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OfferPrice { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}
