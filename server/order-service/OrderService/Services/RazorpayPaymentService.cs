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
        private readonly string? _keyId;
        private readonly string? _keySecret;

        public RazorpayPaymentService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"];
            _keySecret = configuration["Razorpay:KeySecret"];
        }

        public async Task<string> CreateRazorpayOrderAsync(int orderId, decimal amount)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Console.WriteLine($"[RazorpayPaymentService] Initializing Razorpay client with KeyId: {_keyId}");
                    Console.WriteLine($"[RazorpayPaymentService] Creating order for OrderId: {orderId}, Amount: {(int)(amount * 100)} paise");

                    var client = new RazorpayClient(_keyId, _keySecret);
                    var orderData = new Dictionary<string, object>
                    {
                        { "amount", (int)(amount * 100) },
                        { "currency", "INR" },
                        { "receipt", $"order_{orderId}_{DateTime.UtcNow.Ticks}" }
                    };

                    Console.WriteLine($"[RazorpayPaymentService] Sending order creation request: Amount: {orderData["amount"]}, Currency: {orderData["currency"]}, Receipt: {orderData["receipt"]}");

                    var razorpayOrder = client.Order.Create(orderData);
                    Console.WriteLine($"[RazorpayPaymentService] Razorpay order created successfully for OrderId: {orderId}, RazorpayId: {razorpayOrder["id"]}");
                    return razorpayOrder["id"].ToString();
                }
                catch (Exception ex)
                {
                    string errorDetails = ex.Message;
                    if (ex.Data.Contains("error"))
                    {
                        var error = ex.Data["error"] as Dictionary<string, object>;
                        if (error != null)
                        {
                            errorDetails = $"Code: {error.GetValueOrDefault("code", "N/A")}, Description: {error.GetValueOrDefault("description", "N/A")}";
                        }
                    }

                    Console.WriteLine($"[RazorpayPaymentService] Error creating Razorpay order for OrderId: {orderId}, Message: {ex.Message}, Details: {errorDetails}, StackTrace: {ex.StackTrace}");
                    throw new Exception($"Razorpay error for OrderId {orderId}: {errorDetails}", ex);
                }
            });
        }
    }
}