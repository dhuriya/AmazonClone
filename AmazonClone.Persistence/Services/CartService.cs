using AmazonClone.Application.Features.Cart.DTOs;
using AmazonClone.Application.Features.Cart.Interfaces;
using AmazonClone.Domain.Entities;
using AmazonClone.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Persistence.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CartDto> GetMyCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItem)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
            if(cart==null)
            {
                return new CartDto();
            }
            return new CartDto
            {
                Items = cart.CartItem.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity
                }).ToList()
            };
        }
        public async Task<bool> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var cart = await _context.Carts.Include(c => c.CartItem).FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            var cartItem = cart.CartItem.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItem.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateQuantityAsync(string userId, UpdateCartItemDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItem)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
                return false;

            var cartItem = cart.CartItem
                .FirstOrDefault(x => x.ProductId == dto.ProductId);

            if (cartItem == null)
                return false;

            cartItem.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveItemAsync(string userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItem)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
            if (cart == null)
                return false;
            var cartItem = cart.CartItem
                .FirstOrDefault(x => x.ProductId == productId);
            if (cartItem == null)
                return false;
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
