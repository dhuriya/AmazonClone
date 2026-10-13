namespace AmzonClone.Web.Models.Home
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OfferPrice { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}
