using AmazonClone.Application.Features.Wishlist.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Wishlist.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlistAsync(string userId, AddToWishlistDto dto);
        Task<WishlistDto> GetMyWishlistAsync(string userId);
        Task<bool> RemoveFromWishlistAsync(string userId, int productId);
    }
}
