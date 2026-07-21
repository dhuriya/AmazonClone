using AmazonClone.Application.Features.Orders.DTOs;
using AmazonClone.Application.Features.Orders.Interfaces;
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
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> CheckoutAsync(string userId, CreateOrderDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItem)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
            if (cart == null || !cart.CartItem.Any())
                throw new Exception("Cart is Empty");
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                Status = "Pending",
                TotalAmount = 0
            };
            foreach(var item in cart.CartItem)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                });
                order.TotalAmount += item.Product.Price * item.Quantity;
            }
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.CartItem);
            await _context.SaveChangesAsync();
            return new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(x => new OrderItemDto
                {
                    ProductName = x.Product?.Name ?? "",
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList()
            };

        }
        public async Task<List<OrderDto>> GetMyOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .ToListAsync();
            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductName = item.Product.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            }).ToList();
        }
    }
}
