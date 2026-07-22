using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Payments.DTOs
{
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
