using AmazonClone.Application.Features.Cart.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Cart.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetMyCartAsync(string userId);
        Task<bool> AddToCartAsync(string userId, AddToCartDto dto);
        Task<bool> UpdateQuantityAsync(string userId, UpdateCartItemDto dto);
        Task<bool> RemoveItemAsync(string userId, int productId);
    }
}
