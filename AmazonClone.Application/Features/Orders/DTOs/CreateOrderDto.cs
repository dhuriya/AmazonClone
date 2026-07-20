using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Orders.DTOs
{
    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
