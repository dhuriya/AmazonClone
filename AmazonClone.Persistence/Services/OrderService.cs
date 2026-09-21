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
            if (string.IsNullOrWhiteSpace(dto.ShippingAddress))
            {
                throw new BadRequestException("Shipping address is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                throw new BadRequestException("Payment method is required.");
            }

            var cart = await _context.Carts.Include(c => c.CartItem)
                .ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c =>
                    c.UserId == userId && !c.IsDeleted);

            if (cart == null ||
                !cart.CartItem.Any(ci => !ci.IsDeleted))
            {
                throw new BadRequestException("Cart is empty.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var validCartItems = cart.CartItem.Where(ci => !ci.IsDeleted).ToList();
                decimal subTotal = 0;
                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = GenerateOrderNumber(),
                    ShippingAddress = dto.ShippingAddress.Trim(),
                    PaymentMethod = dto.PaymentMethod.Trim(),
                    PaymentStatus = "Pending",
                    Status = OrderStatus.Pending
                };
                order.TrackingHistory.Add(
                new OrderTracking
                {
                    Status = OrderStatus.Pending,
                    Remarks = "Order placed successfully",
                    UpdatedBy = userId
                });
                foreach (var cartItem in validCartItems)
                {
                    var product = cartItem.Product;
                    if (product == null || product.IsDeleted || !product.IsActive)
                    {
                        throw new BadRequestException(
                            $"Product '{cartItem.ProductId}' is no longer available.");
                    }
                    if (cartItem.Quantity <= 0)
                    {
                        throw new BadRequestException(
                            $"Invalid quantity for product '{product.Name}'.");
                    }
                    if (cartItem.Quantity > product.Stock)
                    {
                        throw new BadRequestException(
                            $"Insufficient stock for product '{product.Name}'. " +
                            $"Available stock: {product.Stock}.");
                    }
                    var itemTotal = product.Price * cartItem.Quantity;
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = cartItem.Quantity,
                        TaxAmount = 0,
                        DiscountAmount = 0,
                        TotalAmount = itemTotal
                    };
                    order.OrderItems.Add(orderItem);
                    subTotal += itemTotal;
                    // Deduct stock
                    product.Stock -= cartItem.Quantity;
                }
                order.SubTotal = subTotal;
                // For now tax/shipping/discount are zero.
                // Coupon, GST and shipping module will be added later.
                order.TaxAmount = 0;
                order.ShippingAmount = 0;
                order.DiscountAmount = 0;

                order.TotalAmount = order.SubTotal + order.TaxAmount + order.ShippingAmount
                    - order.DiscountAmount;
                _context.Orders.Add(order);
                // Clear cart after successful order creation
                _context.CartItems.RemoveRange(validCartItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return MapToDto(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<OrderDto>> GetMyOrdersAsync(string userId)
        {
            var orders = await _context.Orders.AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();
            return orders.Select(MapToDto).ToList();
        }
        public async Task<OrderDto?> GetByIdAsync(string userId, int orderId)
        {
            var order = await _context.Orders.AsNoTracking()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId &&
                    o.UserId == userId && !o.IsDeleted);
            if (order == null)
            {
                return null;
            }
            return MapToDto(order);
        }
        public async Task<bool> CancelOrderAsync(string userId, int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId &&
                    o.UserId == userId && !o.IsDeleted);
            if (order == null)
            {
                return false;
            }
            // Customer can cancel only pending orders for now.
            if (order.Status != OrderStatus.Pending)
            {
                return false;
            }
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId &&
                            !p.IsDeleted);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }
                order.Status = OrderStatus.Cancelled;
                order.ModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private static string GenerateOrderNumber()
        {
            return $"AMZ-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                SubTotal = order.SubTotal,
                TaxAmount = order.TaxAmount,
                ShippingAmount = order.ShippingAmount,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                ShippingAddress = order.ShippingAddress,
                CreatedOn = order.CreatedOn,

                Items = order.OrderItems
                    .Select(item => new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TaxAmount = item.TaxAmount,
                        DiscountAmount = item.DiscountAmount,
                        TotalAmount = item.TotalAmount
                    })
                    .ToList()
            };
        }
        public async Task<List<OrderTrackingDto>> GetTrackingAsync(string userId, int orderId)
        {
            var order = await _context.Orders.AsNoTracking().
                FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && !o.IsDeleted);
            if (order == null)
            {
                return new List<OrderTrackingDto>();
            }
            var tracking = await _context.OrderTrackings.AsNoTracking().
                Where(t => t.OrderId == orderId).
                OrderBy(t => t.CreatedOn).Select(t => new OrderTrackingDto
                {
                    Status = t.Status.ToString(),
                    Remarks = t.Remarks,
                    Location = t.Location,
                    UpdatedBy = t.UpdatedBy
                }).ToListAsync();
            return tracking;
        }
        public async Task<bool> UpdateOrderStatusAsync(string userId, int orderId, OrderStatus status, string? remarks, string? location)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
            var currentStatus = order.Status;
            if (!IsValidStatusTransition(currentStatus,status))
            {
                return false;
            }
            order.Status = status;
            order.ModifiedOn = DateTime.UtcNow;
            order.TrackingHistory.Add(new OrderTracking
            {
                Status = status,
                Remarks = remarks ?? $"Order status updated to {status}.",
                Location = location,
                UpdatedBy = userId
            });
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RequestReturnAsync(string userId, int orderId, string reason)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && !o.IsDeleted);
            if (order == null)
            {
                return false; // Return can only be requested for delivered orders
            }
            if(!IsValidStatusTransition(order.Status,OrderStatus.ReturnRequested))
            {
                return false;
            }
            order.Status = OrderStatus.ReturnRequested;
            order.TrackingHistory.Add(new OrderTracking
            {
                OrderId = orderId,
                Status = OrderStatus.ReturnRequested,
                Remarks = reason,
                UpdatedBy = userId,
                CreatedOn = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            if (currentStatus == newStatus)
            {
                return false; // No transition if the status is the same
            }
            return newStatus switch
            {
                OrderStatus.Confirmed => currentStatus == OrderStatus.Pending,
                OrderStatus.Packed => currentStatus == OrderStatus.Confirmed,
                OrderStatus.Shipped => currentStatus == OrderStatus.Packed,
                OrderStatus.InTransit => currentStatus == OrderStatus.Shipped,
                OrderStatus.OutForDelivery => currentStatus == OrderStatus.InTransit,
                OrderStatus.Delivered => currentStatus == OrderStatus.OutForDelivery,
                OrderStatus.Cancelled => currentStatus == OrderStatus.Pending ||
                    currentStatus == OrderStatus.Confirmed,
                OrderStatus.ReturnRequested => currentStatus == OrderStatus.Delivered,
                OrderStatus.Returned => currentStatus == OrderStatus.ReturnRequested,
                OrderStatus.RefundRequested => currentStatus == OrderStatus.Cancelled ||
                    currentStatus == OrderStatus.Returned,
                OrderStatus.Refunded => currentStatus == OrderStatus.RefundRequested,
                OrderStatus.ExchangeRequested => currentStatus == OrderStatus.Delivered,
                OrderStatus.Exchanged => currentStatus == OrderStatus.ExchangeRequested, // Cannot transition from Cancelled to any other status
                _ => false, // Allow all other transitions
            };
        }
    }
}