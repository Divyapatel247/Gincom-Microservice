using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Dtos.Responses;
using OrderService.Models;

namespace OrderService.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentResponseDto ToPaymentResponse(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Status = payment.Status,
                TransactionId = payment.TransactionId
            };
        }
        
    }
}