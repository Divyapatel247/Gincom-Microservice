using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
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
    [ApiController]
    // [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly ProductServiceClient _productService;
        private readonly IPaymentService _paymentService;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(IOrderRepository repository, ProductServiceClient productService, IPaymentService paymentService, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _productService = productService;
            _paymentService = paymentService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userId}")]
        // [Authorize(Policy = "AdminOnly")]

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

            var orderItems = basket.Items.Select(i => new OrderService.Models.OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            Console.WriteLine("[Controller] Mapped Order Items:");
            foreach (var item in orderItems)
            {
                Console.WriteLine($"OrderItem -> ProductId: {item.ProductId}, Quantity: {item.Quantity}");
            }

            // Check stock for all items
            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductAsync(item.ProductId, string.Empty);
                if (product == null)
                    return BadRequest($"Product with ID {item.ProductId} not found");
                if (product.Stock <= 0 || product.Stock < item.Quantity)
                    return BadRequest($"Insufficient stock for Product ID {item.ProductId}. Available: {product.Stock}, Requested: {item.Quantity}");
            }

            // Calculate total amount (for Razorpay)
            decimal totalAmount = 0;
            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductAsync(item.ProductId, string.Empty);
                totalAmount += (product?.Price ?? 0) * item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                Status = "Pending", // Admin will change after payment
                Items = orderItems
            };

            order = await _repository.CreateOrderAsync(order);

            // Initiate Razorpay order (simulating payment)
            var razorpayOrderId = await _paymentService.CreateRazorpayOrderAsync(order.Id, totalAmount * 100); // Amount in paise
            var payment = new Payment
            {
                UserId = userId,
                OrderId = order.Id,
                Status = "Pending", // Will be updated by admin
                TransactionId = razorpayOrderId
            };
            await _repository.CreatePaymentAsync(payment);

            // Publish OrderCreatedEvent (assuming payment is done here for simplicity)
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = userId,
                Items = orderItems.Select(item => new Common.Events.OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList(),
                TotalAmount = totalAmount
            };
            await _publishEndpoint.Publish(orderCreatedEvent);
            Console.WriteLine($"Published OrderCreatedEvent for OrderId: {order.Id} with TotalAmount: {totalAmount}");

            // Clear cart after payment initiation (optional, can be done after confirmation)
            await _repository.ClearBasketAsync(userId);

            return Ok(new { Order = OrderMapper.ToOrderResponse(order), RazorpayOrderId = razorpayOrderId });
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto request)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

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

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _repository.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetOrdersByProductId(int productId)
        {
            var orders = await _repository.GetOrdersByProductIdAsync(productId);
            return Ok(orders);
        }
    }
}