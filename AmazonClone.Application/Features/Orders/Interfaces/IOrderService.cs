using AmazonClone.Application.Features.Orders.DTOs;
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
    }
}
