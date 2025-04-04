using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos.Requests;
using OrderService.Interfaces;
using OrderService.Mappers;

namespace OrderService.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;

        public OrdersController(IOrderRepository repository)
        {
            _repository = repository;
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
            var order = OrderMapper.ToOrder(request);
            order = await _repository.CreateOrderAsync(order);
            return Ok(OrderMapper.ToOrderResponse(order));
        }

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