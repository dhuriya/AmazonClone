using AmazonClone.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
