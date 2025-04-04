using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mysqlx.Crud;
using OrderService.Dtos.Requests;
using OrderService.Dtos.Responses;
using OrderService.Models;

namespace OrderService.Mappers
{
    public static class OrderMapper
    {
        public static OrderService.Models.Order ToOrder(CreateOrderRequestDto request)
        {
            return new OrderService.Models.Order
            {
                ProductId = request.ProductId,
                UserId = request.UserId,
                Status = "Pending"
            };
        }

        public static OrderResponseDto ToOrderResponse(OrderService.Models.Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                UserId = order.UserId,
                Status = order.Status
            };
        }
        
    }
}