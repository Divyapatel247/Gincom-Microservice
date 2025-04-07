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
                UserId = request.UserId,
                Status = "Pending",
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public static OrderResponseDto ToOrderResponse(OrderService.Models.Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Status = order.Status,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}