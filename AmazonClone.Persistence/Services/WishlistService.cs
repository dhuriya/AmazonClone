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
            var wishlist = await _context.Wishlists
                        .Include(w => w.WishlistItems)
                        .FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted);
            if(wishlist==null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }
            if (wishlist.WishlistItems.Any(x => x.ProductId == dto.ProductId))
                return true;
            wishlist.WishlistItems.Add(new WishlistItem
            {
                ProductId = dto.ProductId
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WishlistDto> GetMyWishlistAsync(string userId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId==userId && !w.IsDeleted);
            if(wishlist ==null)
            {
                return new WishlistDto();
            }
            return new WishlistDto
            {
                Items = wishlist.WishlistItems.Select(item => new WishlistItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price
                }).ToList()
            };
        }
        public async Task<bool> RemoveFromWishlistAsync(string userId,int productId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted);
            if (wishlist == null)
                return false;
            var item = wishlist.WishlistItems
                .FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                return false;
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
