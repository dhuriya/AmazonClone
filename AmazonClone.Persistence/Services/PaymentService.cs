using AmazonClone.Application.Features.Payments.DTOs;
using AmazonClone.Application.Features.Payments.Interfaces;
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
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaymentDto> CreatePaymentAsync(string userId,CreatePaymentDto dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId == userId);
            if (order == null)
                throw new Exception("Order not found.");
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                PaymentMethod = dto.PaymentMethod,
                TransactionId = Guid.NewGuid().ToString(),
                Status = "Success"
            };
            _context.Payments.Add(payment);
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                Status = payment.Status
            };
        }
        public async Task<PaymentDto?> GetPaymentAsync(int orderId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && !p.IsDeleted);
            if (payment == null)
                return null;
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                Status = payment.Status
            };
        }
    }
}
