using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Wishlist.DTOs
{
    public class AddToWishlistDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }
}
