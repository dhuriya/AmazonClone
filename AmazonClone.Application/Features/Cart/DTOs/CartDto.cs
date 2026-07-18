using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Cart.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
    }
}
