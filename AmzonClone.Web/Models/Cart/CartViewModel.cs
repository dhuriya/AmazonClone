namespace AmzonClone.Web.Models.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }
}
