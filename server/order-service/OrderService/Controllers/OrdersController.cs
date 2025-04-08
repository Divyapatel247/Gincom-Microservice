using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos.Requests;
using OrderService.Interfaces;
using OrderService.Mappers;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly ProductServiceClient _productService;
        private readonly IPaymentService _paymentService;

        public OrdersController(IOrderRepository repository, ProductServiceClient productService, IPaymentService paymentService)
        {
            _repository = repository;
            _productService = productService;
            _paymentService = paymentService;
        }

        [HttpGet("{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrders(string userId)
        {
            var orders = await _repository.GetOrdersAsync(userId);
            var response = orders.Select(OrderMapper.ToOrderResponse).ToList();
            return Ok(response);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrder(string userId)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null || basket.Items == null || !basket.Items.Any())
                return BadRequest("Cart is empty");

            Console.WriteLine("[Controller] Basket Items:");
            foreach (var i in basket.Items)
            {
                Console.WriteLine($"BasketItem -> Id: {i.Id}, ProductId: {i.ProductId}, Quantity: {i.Quantity}");
            }

            var orderItems = basket.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            Console.WriteLine("[Controller] Mapped Order Items:");
            foreach (var item in orderItems)
            {
                Console.WriteLine($"OrderItem -> ProductId: {item.ProductId}, Quantity: {item.Quantity}");
            }

            // Calculate total amount (for Razorpay)
            decimal totalAmount = 0;
            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductAsync(item.ProductId, null);
                if (product != null)
                {
                    totalAmount += product.Price * item.Quantity;
                }
            }

            // Deduct stock at order creation
            var productQuantities = orderItems
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));
            foreach (var productId in productQuantities.Keys)
            {
                var product = await _productService.GetProductAsync(productId, null);
                if (product == null)
                    return BadRequest($"Product with ID {productId} not found");
                if (!_productService.DeductStock(productId, productQuantities[productId]))
                    return BadRequest($"Insufficient stock for Product ID {productId}. Available: {product.Stock}, Required: {productQuantities[productId]}");
            }

            var order = new Order
            {
                UserId = userId,
                Status = "Pending", // Admin will change
                Items = orderItems
            };

            order = await _repository.CreateOrderAsync(order);

            // Initiate Razorpay order
            var razorpayOrderId = await _paymentService.CreateRazorpayOrderAsync(order.Id, totalAmount * 100); // Amount in paise
            var payment = new Payment
            {
                UserId = userId,
                OrderId = order.Id,
                Status = "Pending", // Admin will change
                TransactionId = razorpayOrderId
            };
            await _repository.CreatePaymentAsync(payment);

            return Ok(new { Order = OrderMapper.ToOrderResponse(order), RazorpayOrderId = razorpayOrderId });
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto request)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            if (request.Status == "Cancelled")
            {
                // Restore stock if order is cancelled
                var productQuantities = order.Items
                    .GroupBy(i => i.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));
                foreach (var productId in productQuantities.Keys)
                {
                    _productService.RestoreStock(productId, productQuantities[productId]);
                }
            }

            order.Status = request.Status; // Admin changes status
            await _repository.UpdateOrderAsync(order);

            var payment = await _repository.GetPaymentByOrderIdAsync(orderId);
            if (payment != null)
            {
                payment.Status = request.Status; // Sync payment status with order
                await _repository.UpdatePaymentAsync(payment);
            }

            return Ok(OrderMapper.ToOrderResponse(order));
        }
    }
}