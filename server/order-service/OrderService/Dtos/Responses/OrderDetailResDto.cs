using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Services;

namespace OrderService.Dtos.Responses
{
   public class OrderDetailResDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public List<OrderItemDetailResDto> Items { get; set; } = new List<OrderItemDetailResDto>();
        public DateTime? CreatedAt { get; set; }
    }

    public class OrderItemDetailResDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ProductDto Product { get; set; }
    }


}