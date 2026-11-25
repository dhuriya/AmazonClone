namespace AmzonClone.Web.Models.Orders
{
    public class CustomerOrderViewModels
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public List<CustomerOrderItemViewModel> Items { get; set; } = new();
    }
}
