using AmazonClone.Application.Features.Orders.DTOs;
using AmazonClone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Orders.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CheckoutAsync(string userId, CreateOrderDto dto);
        Task<List<OrderDto>> GetMyOrdersAsync(string userId);
        Task<bool> CancelOrderAsync(string userId, int orderId);
        Task<OrderDto?> GetByIdAsync(string userId, int orderId);
        Task<List<OrderTrackingDto>> GetTrackingAsync(string userId, int orderId);
        Task<bool> UpdateOrderStatusAsync(string userId,int orderId,OrderStatus status,string? remarks,string? location);
    }
}
