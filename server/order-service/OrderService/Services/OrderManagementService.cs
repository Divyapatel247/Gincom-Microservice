using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using OrderService.Dtos.Requests;
using OrderService.Dtos.Responses;
using OrderService.Interfaces;
using OrderService.Mappers;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderManagementService
    {
        private readonly IOrderRepository _repository;
        private readonly ProductServiceClient _productService;
        private readonly IPaymentService _paymentService;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderManagementService(IOrderRepository repository, ProductServiceClient productService, IPaymentService paymentService, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _productService = productService;
            _paymentService = paymentService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<List<MyOrderResDto>> GetOrdersByUserAsync(string userId)
        {
            var orders = await _repository.GetOrdersAsync(userId);
            if (orders == null || !orders.Any())
                throw new Exception($"No orders found for user {userId}.");

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

            return orderDetailDtos;
        }

        public async Task<(OrderResponseDto Order, string RazorpayOrderId)> CreateOrderAsync(string userId, CreateOrderRequest request)
        {
            Console.WriteLine($"[Service] Starting CreateOrderAsync for userId: {userId}");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.");

            Basket basket;
            try
            {
                basket = await _repository.GetBasketAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service] Error retrieving basket for userId {userId}: {ex.Message}");
                throw new Exception($"Failed to retrieve cart for user {userId}: {ex.Message}");
            }

            Console.WriteLine($"[Service] Retrieved basket for userId: {userId}, Basket: {basket?.Id}, Items count: {basket?.Items?.Count ?? 0}");
            if (basket == null || basket.Items == null || !basket.Items.Any())
            {
                Console.WriteLine($"[Service] Error: Cart is empty for userId: {userId}");
                throw new Exception("Cart is empty");
            }

            Console.WriteLine("[Service] Basket Items:");
            foreach (var i in basket.Items)
            {
                Console.WriteLine($"BasketItem -> Id: {i.Id}, ProductId: {i.ProductId}, Quantity: {i.Quantity}");
            }

            var orderItems = basket.Items.Select(i => new Models.OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            Console.WriteLine("[Service] Mapped Order Items:");
            foreach (var item in orderItems)
            {
                Console.WriteLine($"OrderItem -> ProductId: {item.ProductId}, Quantity: {item.Quantity}");
            }

            // Check stock for all items
            foreach (var item in orderItems)
            {
                Console.WriteLine($"[Service] Checking stock for ProductId: {item.ProductId}, Quantity: {item.Quantity}");
                var product = await _productService.GetProductAsync(item.ProductId, string.Empty);
                if (product == null)
                {
                    Console.WriteLine($"[Service] Error: Product not found for ProductId: {item.ProductId}");
                    throw new Exception($"Product with ID {item.ProductId} not found");
                }
                if (product.Stock <= 0 || product.Stock < item.Quantity)
                {
                    Console.WriteLine($"[Service] Error: Insufficient stock for ProductId: {item.ProductId}, Available: {product.Stock}, Requested: {item.Quantity}");
                    throw new Exception($"Insufficient stock for Product ID {item.ProductId}. Available: {product.Stock}, Requested: {item.Quantity}");
                }
                Console.WriteLine($"[Service] Stock check passed for ProductId: {item.ProductId}, Available: {product.Stock}");
            }

            // Calculate total amount (for Razorpay)
            decimal totalAmount = 0;
            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductAsync(item.ProductId, string.Empty);
                totalAmount += (product?.Price ?? 0) * item.Quantity;
                Console.WriteLine($"[Service] Adding to total: ProductId: {item.ProductId}, Price: {product?.Price}, Quantity: {item.Quantity}, Subtotal: {(product?.Price ?? 0) * item.Quantity}");
            }
            Console.WriteLine($"[Service] Total amount calculated: {totalAmount}");

            var order = new Order
            {
                UserId = userId,
                Status = "Pending",
                Items = orderItems
            };

            try
            {
                order = await _repository.CreateOrderAsync(order);
                Console.WriteLine($"[Service] Order created with Id: {order.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service] Error creating order for userId {userId}: {ex.Message}");
                throw new Exception($"Failed to create order: {ex.Message}");
            }

            // Initiate Razorpay order (simulating payment)
            string razorpayOrderId;
            try
            {
                razorpayOrderId = await _paymentService.CreateRazorpayOrderAsync(order.Id, totalAmount * 100);
                Console.WriteLine($"[Service] Razorpay order created with Id: {razorpayOrderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service] Error creating Razorpay order for OrderId {order.Id}: {ex.Message}");
                throw new Exception($"Failed to initiate payment with Razorpay: {ex.Message}");
            }

            try
            {
                var payment = new Payment
                {
                    UserId = userId,
                    OrderId = order.Id,
                    Status = "Pending",
                    TransactionId = razorpayOrderId
                };
                await _repository.CreatePaymentAsync(payment);
                Console.WriteLine($"[Service] Payment record created for OrderId: {order.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service] Error creating payment record for OrderId {order.Id}: {ex.Message}");
                throw new Exception($"Failed to create payment record: {ex.Message}");
            }

            // Publish OrderCreatedEvent
            try
            {
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
                Console.WriteLine($"[Service] Published OrderCreatedEvent for OrderId: {order.Id} with TotalAmount: {totalAmount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service] Error publishing OrderCreatedEvent for OrderId {order.Id}: {ex.Message}");
                throw new Exception($"Failed to publish order event: {ex.Message}");
            }

            // Clear cart after payment initiation
            try
            {
                await _repository.ClearBasketAsync(userId);
                Console.WriteLine($"[Service] Cleared cart for userId: {userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service] Error clearing cart for userId {userId}: {ex.Message}");
                throw new Exception($"Failed to clear cart: {ex.Message}");
            }

            return (OrderMapper.ToOrderResponse(order), razorpayOrderId);
        }
        public async Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDto request)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new Exception($"Order with ID {orderId} not found.");

            var oldStatus = order.Status;
            order.Status = request.Status;
            await _repository.UpdateOrderAsync(order);

            var payment = await _repository.GetPaymentByOrderIdAsync(orderId);
            if (payment != null)
            {
                payment.Status = request.Status;
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

            return OrderMapper.ToOrderResponse(order);
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
                throw new Exception("No orders found.");

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

                decimal orderTotal = 0;
                foreach (var item in order.Items)
                {
                    var product = await _productService.GetProductAsync(item.ProductId);
                    decimal price = product?.Price ?? 0;
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

            return orderResponseDtos;
        }

        public async Task<List<Order>> GetOrdersByProductIdAsync(int productId)
        {
            return await _repository.GetOrdersByProductIdAsync(productId);
        }

        public async Task<OrderDetailResDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new Exception($"Order with ID {orderId} not found.");

            var orderDetailDto = new OrderDetailResDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Status = order.Status,
                Items = new List<OrderItemDetailResDto>(),
                CreatedAt = order.CreatedAt
            };

            foreach (var item in order.Items)
            {
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

            return orderDetailDto;
        }
    }
}