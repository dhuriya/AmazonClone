using AmazonClone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Orders.DTOs
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
        public string? Remarks { get; set; }
        public string? Location { get; set; }
    }
}
