using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Wishlist.DTOs
{
    public class WishlistDto
    {
        public List<WishlistItemDto> Items { get; set; } = new();
    }
}
