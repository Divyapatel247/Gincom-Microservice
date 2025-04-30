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
        private readonly OrderManagementService _orderService;

        public OrdersController(OrderManagementService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("user/{userId}")]
        // [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrdersByUser(string userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrder(string userId, [FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    ModelState.AddModelError("userId", "User ID is required.");
                    return BadRequest(ModelState);
                }

                var (order, razorpayOrderId) = await _orderService.CreateOrderAsync(userId, request);
                return Ok(new { Order = order, RazorpayOrderId = razorpayOrderId });
            }
            catch (Exception ex)
            {
                // Log the exception and return a specific message
                Console.WriteLine($"Error in CreateOrder: {ex.Message}");
                return BadRequest(new { error = ex.Message }); // Return the specific exception message
            }
        }
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto request)
        {
            try
            {
                var response = await _orderService.UpdateOrderStatusAsync(orderId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetOrdersByProductId(int productId)
        {
            var orders = await _orderService.GetOrdersByProductIdAsync(productId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}