using AmazonClone.Domain.Common;
using AmazonClone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Entities
{
    public class OrderTracking : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null;
        public OrderStatus Status { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
