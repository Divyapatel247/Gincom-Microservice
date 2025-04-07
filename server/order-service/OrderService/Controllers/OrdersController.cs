using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly ProductServiceClient _productService;

        public OrdersController(IOrderRepository repository, ProductServiceClient productService)
        {
            _repository = repository;
            _productService = productService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrders(string userId)
        {
            // Ensure the requesting user matches the userId (or is admin)
            // if (User.Identity.Name != userId && !User.IsInRole("Admin"))
            //     return Forbid();

            var orders = await _repository.GetOrdersAsync(userId);
            var response = orders.Select(OrderMapper.ToOrderResponse).ToList();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest("UserId is required");

            var basket = await _repository.GetBasketAsync(request.UserId);

            if (basket == null || basket.Items == null || !basket.Items.Any())
                return BadRequest("Cart is empty");

            Console.WriteLine("[Controller] Basket Items:");
            foreach (var i in basket.Items)
            {
                Console.WriteLine($"BasketItem -> Id: {i.Id}, ProductId: {i.ProductId}, Quantity: {i.Quantity}");
            }

            var productQuantities = basket.Items
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            foreach (var productId in productQuantities.Keys)
            {
                var product = await _productService.GetProductAsync(productId, null);
                if (product == null)
                    return BadRequest($"Product with ID {productId} not found");

                // if (!_productService.ReserveStock(productId, productQuantities[productId]))
                //     return BadRequest($"Insufficient stock for Product ID {productId}. Available: {product.Stock}, Required: {productQuantities[productId]}");
                if (product.Stock < productQuantities[productId])
                    return BadRequest($"Insufficient stock...");
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

            var order = new Order
            {
                UserId = request.UserId,
                Status = "Pending",
                Items = orderItems
            };

            order = await _repository.CreateOrderAsync(order);

            foreach (var item in order.Items)
            {
                _productService.CommitStock(item.ProductId, item.Quantity);
            }

            await _repository.ClearBasketAsync(request.UserId);

            return Ok(OrderMapper.ToOrderResponse(order));
        }


        // [HttpPost]
        // public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        // {
        //     if (string.IsNullOrEmpty(request.UserId))
        //         return BadRequest("UserId is required");

        //     if (request.Items == null || !request.Items.Any())
        //         return BadRequest("Order must contain at least one item");

        //     var productQuantities = request.Items
        //         .GroupBy(i => i.ProductId)
        //         .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        //     foreach (var productId in productQuantities.Keys)
        //     {
        //         var product = await _productService.GetProductAsync(productId, null);
        //         if (product == null)
        //             return BadRequest($"Product with ID {productId} not found");

        //         if (!_productService.ReserveStock(productId, productQuantities[productId]))
        //             return BadRequest($"Insufficient stock for Product ID {productId}. Available: {product.Stock}, Required: {productQuantities[productId]}");
        //     }

        //     var order = new Order
        //     {
        //         UserId = request.UserId,
        //         Status = "Pending",
        //         Items = request.Items.Select(i => new OrderItem
        //         {
        //             ProductId = i.ProductId,
        //             Quantity = i.Quantity
        //         }).ToList()
        //     };

        //     order = await _repository.CreateOrderAsync(order);

        //     foreach (var item in order.Items)
        //     {
        //         _productService.CommitStock(item.ProductId, item.Quantity);
        //     }

        //     return Ok(OrderMapper.ToOrderResponse(order));
        // }



        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto request)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            order.Status = request.Status;
            await _repository.UpdateOrderAsync(order);
            return Ok(OrderMapper.ToOrderResponse(order));
        }

    }
}