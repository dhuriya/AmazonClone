namespace AmzonClone.Web.Models.Home
{
    public class HomeViewModel
    {
        public List<CategoryViewModel> Categories { get; set; } = new();
        public List<ProductViewModel> FeaturedProducts { get; set; } = new();
        public List<ProductViewModel> TrendingProducts { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
