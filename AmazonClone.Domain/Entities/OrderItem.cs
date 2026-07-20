using AmazonClone.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
