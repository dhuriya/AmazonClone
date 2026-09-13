using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Orders.DTOs
{
    public class OrderDto
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

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
