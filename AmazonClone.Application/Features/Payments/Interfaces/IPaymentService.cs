using AmazonClone.Application.Features.Payments.DTOs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Payments.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(string userId, CreatePaymentDto dto);
        Task<PaymentDto?> GetPaymentAsync(int orderId);
    }
}
