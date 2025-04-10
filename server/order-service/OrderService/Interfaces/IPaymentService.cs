using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateRazorpayOrderAsync(int orderId, decimal amount);
    }
}