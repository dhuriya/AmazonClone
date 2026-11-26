namespace AmzonClone.Web.Models.Wishlist
{
    public class WishlistItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
    }
}
