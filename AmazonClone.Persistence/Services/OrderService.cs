using AmazonClone.Application.Common.Exceptions;
using AmazonClone.Application.Features.Orders.DTOs;
using AmazonClone.Application.Features.Orders.Interfaces;
using AmazonClone.Domain.Entities;
using AmazonClone.Domain.Enums;
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
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    !c.IsDeleted);

            if (cart == null || !cart.CartItem.Any())
                throw new BadRequestException("Cart is Empty");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddress = dto.ShippingAddress,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0
                };

                foreach (var item in cart.CartItem)
                {
                    var product = item.Product;

                    if (product == null ||
                        product.IsDeleted ||
                        !product.IsActive)
                    {
                        throw new BadRequestException(
                            $"Product '{item.ProductId}' is no longer available.");
                    }

                    if (item.Quantity <= 0)
                    {
                        throw new BadRequestException(
                            $"Invalid quantity for product '{product.Name}'.");
                    }

                    if (item.Quantity > product.Stock)
                    {
                        throw new BadRequestException(
                            $"Insufficient stock for product '{product.Name}'. " +
                            $"Available stock: {product.Stock}.");
                    }

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Price = product.Price,
                        Quantity = item.Quantity
                    });

                    order.TotalAmount += product.Price * item.Quantity;

                    product.Stock -= item.Quantity;
                }

                _context.Orders.Add(order);

                _context.CartItems.RemoveRange(cart.CartItem);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new OrderDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    ShippingAddress = order.ShippingAddress,
                    Items = order.OrderItems.Select(x => new OrderItemDto
                    {
                        ProductName = x.Product?.Name ?? string.Empty,
                        Price = x.Price,
                        Quantity = x.Quantity
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
        public async Task<bool> CancelOrderAsync(string userId, int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && !o.IsDeleted);
            if(order == null)
            {
                return false;
            }
            if(order.Status != OrderStatus.Pending)
            {
                return false;
            }
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if(product != null && !product.IsDeleted)
                {
                    product.Stock += item.Quantity;
                }
            }
            order.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<OrderDto?> GetByIdAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && !o.IsDeleted);
            if (order == null)
            {
                return null;
            }
            return new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductName = item.Product?.Name ?? string.Empty,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}
