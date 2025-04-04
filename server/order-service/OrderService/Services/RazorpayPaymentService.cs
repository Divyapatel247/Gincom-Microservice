using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Interfaces;
using Razorpay.Api;

namespace OrderService.Services
{
    public class RazorpayPaymentService : IPaymentService
    {
        private readonly string _keyId;
        private readonly string _keySecret;

        public RazorpayPaymentService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"];
            _keySecret = configuration["Razorpay:KeySecret"];
        }

        public async Task<string> CreateRazorpayOrderAsync(int orderId, decimal amount)
        {
            var client = new RazorpayClient(_keyId, _keySecret);
            var orderData = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) }, 
                { "currency", "INR" },
                { "receipt", $"order_{orderId}" }
            };
            var razorpayOrder = client.Order.Create(orderData);
            return razorpayOrder["id"].ToString();
        
    }
}
}