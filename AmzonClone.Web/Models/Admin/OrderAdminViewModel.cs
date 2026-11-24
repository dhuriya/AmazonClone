namespace AmzonClone.Web.Models.Admin
{
    public class OrderAdminViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public List<OrderItemAdminViewModel> Items { get; set; } = new();
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
