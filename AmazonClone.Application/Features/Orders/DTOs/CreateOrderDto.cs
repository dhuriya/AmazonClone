using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Orders.DTOs
{
    public class CreateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int AddressId { get; set; }
        //public string ShippingAddress { get; set; } = string.Empty;
        [Required]
        public string PaymentMethod { get; set; } = "COD";
    }
}
