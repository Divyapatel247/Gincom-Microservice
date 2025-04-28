using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos.Requests;
using OrderService.Dtos.Responses;
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

        [HttpGet("user/{userId}")]
        // [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrdersByUser(string userId)
        {
            try
            {
                var orders = await _repository.GetOrdersAsync(userId);
                if (orders == null || !orders.Any())
                {
                    return NotFound($"No orders found for user {userId}.");
                }

                var orderDetailDtos = new List<MyOrderResDto>();

                foreach (var order in orders)
                {
                    var orderDetailDto = new MyOrderResDto
                    {
                        Id = order.Id,
                        Status = order.Status,
                        Items = new List<MyOrderItemDetailResDto>(),
                        CreatedAt = order.CreatedAt
                    };

                    // Fetch product details for each order item
                    foreach (var item in order.Items)
                    {
                        var productDetail = await _productService.GetProductAsync(item.ProductId);
                        orderDetailDto.Items.Add(new MyOrderItemDetailResDto
                        {
                            Id = item.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Product = productDetail ?? new ProductDto { Id = item.ProductId, Title = "Unknown Product" }
                        });
                    }

                    orderDetailDtos.Add(orderDetailDto);
                }

                return Ok(orderDetailDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrder(string userId,[FromBody] CreateOrderRequest request)
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
                Email = request.UserEmail,
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
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }
            var oldStatus = order.Status;
            order.Status = request.Status;
            await _repository.UpdateOrderAsync(order);

            var payment = await _repository.GetPaymentByOrderIdAsync(orderId);
            if (payment != null)
            {
                payment.Status = request.Status; // Sync payment status with order
                await _repository.UpdatePaymentAsync(payment);
            }

            var orderStatusUpdatedEvent = new OrderStatusUpdatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OldStatus = oldStatus,
                NewStatus = order.Status,
                UpdatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(orderStatusUpdatedEvent);
            Console.WriteLine($"Published OrderStatusUpdatedEvent for OrderId: {order.Id} from {oldStatus} to {order.Status}");

            return Ok(OrderMapper.ToOrderResponse(order));
        }
      [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _repository.GetAllOrdersAsync();
                if (orders == null || !orders.Any())
                {
                    return NotFound("No orders found.");
                }

                var orderResponseDtos = new List<OrderResponseDto>();

                foreach (var order in orders)
                {
                    var orderResponseDto = new OrderResponseDto
                    {
                        Id = order.Id,
                        UserId = order.UserId,
                        Status = order.Status,
                        Items = new List<OrderItemResponseDto>(),
                        TotalAmount = 0,
                        CreatedAt = order.CreatedAt
                    };

                    // Fetch product prices and calculate total
                    decimal orderTotal = 0;
                    foreach (var item in order.Items)
                    {
                        var product = await _productService.GetProductAsync(item.ProductId);
                        decimal price = product?.Price ?? 0; // Use 0 if product fetch fails
                        orderTotal += price * item.Quantity;

                        orderResponseDto.Items.Add(new OrderItemResponseDto
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = price
                        });
                    }

                    orderResponseDto.TotalAmount = orderTotal;
                    orderResponseDtos.Add(orderResponseDto);
                }

                return Ok(orderResponseDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetOrdersByProductId(int productId)
        {
            var orders = await _repository.GetOrdersByProductIdAsync(productId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _repository.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                var orderDetailDto = new OrderDetailResDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Status = order.Status,
                    Items = new List<OrderItemDetailResDto>(),
                    CreatedAt = order.CreatedAt
                };

                // Fetch product details for each order item
                foreach (var item in order.Items)
                {
                    // Pass token as null for now; adjust if authentication is required
                    var productDetail = await _productService.GetProductAsync(item.ProductId, token: null);
                    orderDetailDto.Items.Add(new OrderItemDetailResDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        CreatedAt = item.CreatedAt,
                        Product = productDetail ?? new ProductDto { Id = item.ProductId, Title = "Unknown Product" }
                    });
                }

                return Ok(orderDetailDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}

public class CreateOrderRequest
{
    public string UserEmail { get; set; } = string.Empty;
}
