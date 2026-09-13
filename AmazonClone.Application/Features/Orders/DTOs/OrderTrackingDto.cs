using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Orders.DTOs
{
    public class OrderTrackingDto
    {
        public string Status { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
