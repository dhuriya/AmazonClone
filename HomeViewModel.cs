public class HomeViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new();
    public List<ProductViewModel> FeaturedProducts { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
