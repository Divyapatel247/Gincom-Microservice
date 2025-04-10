using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
    public class CreateOrderRequestDto
    {
        public string UserId { get; set; }
        public List<OrderItemDto>? Items { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}