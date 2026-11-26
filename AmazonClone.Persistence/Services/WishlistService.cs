using AmazonClone.Application.Features.Wishlist.DTOs;
using AmazonClone.Application.Features.Wishlist.Interfaces;
using AmazonClone.Domain.Entities;
using AmazonClone.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Persistence.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;
        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddToWishlistAsync(string userId, AddToWishlistDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId &&
                    !p.IsDeleted && p.IsActive && p.Stock > 0);
            if (product is null)
                return false;
            var wishlist = await _context.Wishlists.Include(w => w.WishlistItems).FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted);
            if (wishlist is null)
            {
                wishlist = new Wishlist { UserId = userId };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }
            var existingItem = wishlist.WishlistItems.FirstOrDefault(item => item.ProductId == dto.ProductId);
            if (existingItem is not null)
            {
                if (existingItem.IsDeleted)
                {
                    existingItem.IsDeleted = false;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            wishlist.WishlistItems.Add(new WishlistItem
            {
                ProductId = dto.ProductId
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WishlistDto> GetMyWishlistAsync(string userId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.WishlistItems).ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted);
            if (wishlist is null)
                return new WishlistDto();
            return new WishlistDto
            {
                Items = wishlist.WishlistItems.Where(item => !item.IsDeleted &&
                        item.Product != null && !item.Product.IsDeleted)
                    .Select(item => new WishlistItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Price = item.Product.Price,
                        ImageUrl = item.Product.ImageUrl,
                        Stock = item.Product.Stock,
                        IsAvailable = item.Product.IsActive && item.Product.Stock > 0
                    })
                    .ToList()
            };
        }
        public async Task<bool> RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted);
            if (wishlist is null)
                return false;
            var item = wishlist.WishlistItems.FirstOrDefault(x => x.ProductId == productId && !x.IsDeleted);
            if (item is null)
                return false;
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
