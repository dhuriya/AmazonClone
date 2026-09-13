using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Packed = 3,
        Shipped = 4,
        InTransit = 5,
        OutForDelivery = 6,
        Delivered = 7,
        Cancelled = 8,
        ReturnRequested = 9,
        Returned = 10,
        RefundRequested = 11,
        Refunded = 12,
        ExchangeRequested = 13,
        Exchanged = 14
    }
}
