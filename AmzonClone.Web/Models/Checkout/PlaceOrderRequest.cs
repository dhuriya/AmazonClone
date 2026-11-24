namespace AmzonClone.Web.Models.Checkout
{
    public class PlaceOrderRequest
    {
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = "COD";
    }
}
